using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using DaemonsMCP.Application.FileSystem.Services;
using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Repositories;

namespace DaemonsMCP.Infrastructure.Services { 

  public class FileSystemSyncService : IFileSystemSyncService {
    private readonly IFileSystemNodeRepository _nodeRepository;
    private readonly ISettingRepository _settingRepository;
    private readonly IIndexQueueRepository _indexQueueRepository;

    public FileSystemSyncService(
        IFileSystemNodeRepository nodeRepository,
        ISettingRepository settingRepository,
        IIndexQueueRepository indexQueueRepository) {
      _nodeRepository = nodeRepository;
      _settingRepository = settingRepository;
      _indexQueueRepository = indexQueueRepository;
    }

    public async Task<SyncResult> SyncProjectAsync(
        DaemonsMCP.Domain.Entities.Project project,
        CancellationToken cancellationToken = default) {
      var stopwatch = Stopwatch.StartNew();

      // Validate root path exists
      if (!Directory.Exists(project.RootPath)) {
        throw new DirectoryNotFoundException($"Root path does not exist: {project.RootPath}");
      }

      // Load filter settings
      var filters = await LoadFiltersAsync(cancellationToken);

      // Step 1: Load existing DB state into dictionary for fast lookup
      var existingNodes = await _nodeRepository.GetByProjectIdAsync(project.Id, cancellationToken);
      var existingByPath = existingNodes.ToDictionary(n => n.RelativePath, StringComparer.OrdinalIgnoreCase);

      // Step 2: Scan filesystem and build what SHOULD exist
      var filesystemState = ScanFileSystem(project, project.RootPath, filters);
      var filesystemByPath = filesystemState.ToDictionary(n => n.RelativePath, StringComparer.OrdinalIgnoreCase);

      // Step 3: Determine what changed
      var toAdd = new List<FileSystemNode>();
      var toUpdate = new List<FileSystemNode>();
      var toDelete = new List<FileSystemNode>();

      // Find additions and updates
      foreach (var fsNode in filesystemState) {
        if (existingByPath.TryGetValue(fsNode.RelativePath, out var existingNode)) {
          // Node exists - check if it needs updating
          if (NeedsUpdate(existingNode, fsNode)) {
            existingNode.UpdateMetadata(fsNode.SizeInBytes, fsNode.ModifiedAt);
            toUpdate.Add(existingNode);
          }
        } else {
          // New node
          toAdd.Add(fsNode);
        }
      }

      // Find deletions
      foreach (var existingNode in existingNodes) {
        if (!filesystemByPath.ContainsKey(existingNode.RelativePath)) {
          toDelete.Add(existingNode);
        }
      }

      // Step 4: Apply changes in correct order
      // Delete files first (bottom-up), then directories
      var filesToDelete = toDelete.Where(n => !n.IsDirectory).ToList();
      var dirsToDelete = toDelete.Where(n => n.IsDirectory).OrderByDescending(n => n.RelativePath.Length).ToList();

      foreach (var node in filesToDelete) {
        await _nodeRepository.DeleteAsync(node, cancellationToken);
      }

      foreach (var node in dirsToDelete) {
        await _nodeRepository.DeleteAsync(node, cancellationToken);
      }

      // Add directories first (top-down), then files
      var dirsToAdd = toAdd.Where(n => n.IsDirectory).OrderBy(n => n.RelativePath.Length).ToList();
      var filesToAdd = toAdd.Where(n => !n.IsDirectory).ToList();

      // We need to assign ParentIds as we go for new directories
      await AddNodesWithParentResolution(project.Id, dirsToAdd, existingByPath, cancellationToken);
      await AddNodesWithParentResolution(project.Id, filesToAdd, existingByPath, cancellationToken);

      // Update existing nodes
      foreach (var node in toUpdate) {
        await _nodeRepository.UpdateAsync(node, cancellationToken);
      }

      // Step 5: Save all changes
      await _nodeRepository.SaveChangesAsync(cancellationToken);

      stopwatch.Stop();

      return new SyncResult(
          FilesAdded: filesToAdd.Count,
          FilesUpdated: toUpdate.Count(n => !n.IsDirectory),
          FilesDeleted: filesToDelete.Count,
          DirectoriesAdded: dirsToAdd.Count,
          DirectoriesDeleted: dirsToDelete.Count,
          Duration: stopwatch.Elapsed
      );
    }

    private async Task<FileSystemFilters> LoadFiltersAsync(CancellationToken cancellationToken) {
      var settings = await _settingRepository.GetAllAsDictionaryAsync(cancellationToken);

      var blockedFolders = ParseCommaSeparated(settings.GetValueOrDefault("FileSystem.BlockedFolders", ""));
      var blockedExtensions = ParseCommaSeparated(settings.GetValueOrDefault("FileSystem.BlockedExtensions", ""));
      var allowedExtensions = ParseCommaSeparated(settings.GetValueOrDefault("FileSystem.AllowedExtensions", ""));
      var blockedFiles = ParseCommaSeparated(settings.GetValueOrDefault("FileSystem.BlockedFiles", ""));

      return new FileSystemFilters(
          blockedFolders,
          blockedExtensions,
          allowedExtensions,
          blockedFiles
      );
    }

    private HashSet<string> ParseCommaSeparated(string value) {
      if (string.IsNullOrWhiteSpace(value)) {
        return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
      }

      return value
          .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
          .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private List<FileSystemNode> ScanFileSystem(DaemonsMCP.Domain.Entities.Project project, string rootPath, FileSystemFilters filters) {
      var nodes = new List<FileSystemNode>();
      var rootDir = new DirectoryInfo(rootPath);

      // Recursively scan
      ScanDirectory(project, rootDir, rootPath, nodes, filters);

      return nodes;
    }

    private void ScanDirectory(
        DaemonsMCP.Domain.Entities.Project project,
        DirectoryInfo directory,
        string rootPath,
        List<FileSystemNode> nodes,
        FileSystemFilters filters) {
      try {
        // Check if this directory should be blocked
        if (filters.BlockedFolders.Contains(directory.Name) || (!IsPathSafe(directory.FullName, project, out var errorStr))) {
          return; // Skip this entire directory tree
        }

        // Calculate relative path
        var relativePath = GetRelativePath(rootPath, directory.FullName);

        // Add directory node (skip root itself - it's the project)
        if (!string.IsNullOrEmpty(relativePath)) {
          var dirNode = FileSystemNode.CreateDirectory(
              project.Id,
              directory.Name,
              relativePath,
              parentId: null  // We'll resolve this later
          );

          // Set timestamps from actual directory
          typeof(FileSystemNode)
              .GetProperty("CreatedAt")!
              .SetValue(dirNode, directory.CreationTimeUtc);
          typeof(FileSystemNode)
              .GetProperty("ModifiedAt")!
              .SetValue(dirNode, directory.LastWriteTimeUtc);

          nodes.Add(dirNode);
        }

        // Scan files in this directory
        foreach (var file in directory.GetFiles()) {
          // Apply file filters
          if (filters.BlockedFiles.Contains(file.Name)) {
            continue; // Skip blocked files
          }

          var extension = file.Extension?.ToLowerInvariant() ?? string.Empty;

          // Check blocked extensions
          if (filters.BlockedExtensions.Contains(extension)) {
            continue; // Skip blocked extensions
          }

          // Check allowed extensions (if whitelist is set)
          if (filters.AllowedExtensions.Count > 0 &&
              !filters.AllowedExtensions.Contains(extension)) {
            continue; // Not in whitelist
          }

          var fileRelativePath = GetRelativePath(rootPath, file.FullName);
          var extensionWithoutDot = extension.TrimStart('.');

          var fileNode = FileSystemNode.CreateFile(
              project.Id,
              file.Name,
              fileRelativePath,
              file.Length,
              extensionWithoutDot,
              parentId: null  // We'll resolve this later
          );

          // Set timestamps from actual file
          typeof(FileSystemNode)
              .GetProperty("CreatedAt")!
              .SetValue(fileNode, file.CreationTimeUtc);
          typeof(FileSystemNode)
              .GetProperty("ModifiedAt")!
              .SetValue(fileNode, file.LastWriteTimeUtc);

          nodes.Add(fileNode);
        }

        // Recursively scan subdirectories
        foreach (var subDir in directory.GetDirectories()) {
          ScanDirectory(project, subDir, rootPath, nodes, filters);
        }
      } catch (UnauthorizedAccessException) {
        // Skip directories we can't access
      }
    }

    /// <summary>
    /// Validates that the file path is safe for write operations (prevents directory traversal attacks).
    /// </summary>
    /// <param name="filePath">The file path to validate</param>
    /// <returns>True if the path is safe, false otherwise</returns>
    private bool IsPathSafe(string filePath, DaemonsMCP.Domain.Entities.Project project, out string error) {
      error = string.Empty;
      try {
        // Check for directory traversal attempts
        if (filePath.Contains("..") || filePath.Contains("~/")) {
          error = $"Directory traversal attempt detected: {filePath}";
          return false;
        }

        // Additional checks for suspicious patterns
        var suspiciousPatterns = new[] { "%", "$", "`" };
        if (suspiciousPatterns.Any(pattern => filePath.Contains(pattern))) {
          error = $"Suspicious pattern detected in path: {filePath}";
          return false;
        }

        // Ensure the path can be properly normalized
        var checkPath = Path.GetFullPath(filePath);

        // Check for absolute paths that might escape project boundaries
        if (Path.IsPathRooted(filePath)) {
          // Allow only if it's within a configured project path
          var boundryCheck = filePath.StartsWith(project.RootPath, StringComparison.OrdinalIgnoreCase);
          if (!boundryCheck) {
            error = $"final rooted boundy check false";
          }
          return boundryCheck;
        }

        return true;
      } catch (Exception e) {
        // If path normalization fails, it's not safe
        error = $"IsPathSafe exception {filePath} {e.Message}";
        return false;
      }
    }

    private string GetRelativePath(string rootPath, string fullPath) {
      var root = new Uri(rootPath.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar);
      var full = new Uri(fullPath);
      var relative = root.MakeRelativeUri(full).ToString();
      return Uri.UnescapeDataString(relative).Replace('/', Path.DirectorySeparatorChar);
    }

    private bool NeedsUpdate(FileSystemNode existing, FileSystemNode filesystem) {
      // For files, check size and modified date
      if (!existing.IsDirectory) {
        return existing.SizeInBytes != filesystem.SizeInBytes ||
               existing.ModifiedAt != filesystem.ModifiedAt;
      }

      // For directories, check modified date
      return existing.ModifiedAt != filesystem.ModifiedAt;
    }

    private async Task AddNodesWithParentResolution(
        int projectId,
        List<FileSystemNode> nodes,
        Dictionary<string, FileSystemNode> existingByPath,
        CancellationToken cancellationToken) {
      // Group by depth level (number of path separators)
      var nodesByDepth = nodes
          .GroupBy(n => n.RelativePath.Count(c => c == Path.DirectorySeparatorChar))
          .OrderBy(g => g.Key)
          .ToList();

      // Process level by level so parents always exist before children
      foreach (var level in nodesByDepth) {
        foreach (var node in level) {
          // Resolve parent ID by finding parent directory path
          var parentPath = GetParentPath(node.RelativePath);

          if (!string.IsNullOrEmpty(parentPath)) {
            // Check if parent exists in DB (including nodes we just added)
            if (existingByPath.TryGetValue(parentPath, out var parentNode)) {
              // Use reflection to set ParentId since it's private
              typeof(FileSystemNode)
                  .GetProperty("ParentId")!
                  .SetValue(node, parentNode.Id);
            }
          }

          var addedNode = await _nodeRepository.AddAsync(node, cancellationToken);       
        }

        // Save after each level so new nodes get IDs for the next level
        await _nodeRepository.SaveChangesAsync(cancellationToken);

        // Update lookup with newly added nodes
        foreach (var node in level) {
          existingByPath[node.RelativePath] = node;
          // If it's a code file (.cs), queue it for indexing
          if (!node.IsDirectory && node.Extension == "cs") {
            var queueItem = IndexQueue.Create(projectId, node.Id, node.RelativePath);
            await _indexQueueRepository.AddAsync(queueItem);
          }
        }
      }
    }

    private string? GetParentPath(string relativePath) {
      var lastSeparator = relativePath.LastIndexOf(Path.DirectorySeparatorChar);

      if (lastSeparator <= 0) {
        return null;  // Root level item
      }

      return relativePath.Substring(0, lastSeparator);
    }
  }


  // Helper record for filter settings
  internal record FileSystemFilters(
      HashSet<string> BlockedFolders,
      HashSet<string> BlockedExtensions,
      HashSet<string> AllowedExtensions,
      HashSet<string> BlockedFiles
  );

}