using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Infrastructure.Repositories;

public class FileSystemNodeRepository : IFileSystemNodeRepository {
  private readonly DaemonsMcpDbContext _context;
  private readonly ILogger<FileSystemNodeRepository> _logger;

  public FileSystemNodeRepository(DaemonsMcpDbContext context, ILoggerFactory loggerFactory ) {
    _context = context;
    _logger = loggerFactory.CreateLogger<FileSystemNodeRepository>();
  }

  public async Task<FileSystemNode?> GetByIdAsync(int id, CancellationToken cancellationToken = default) {
    return await _context.FileSystemNodes
        .Include(f => f.Children)
        .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
  }

  public async Task<FileSystemNode?> GetByPathAsync(int projectId, string relativePath, CancellationToken cancellationToken = default) {
    return await _context.FileSystemNodes
        .FirstOrDefaultAsync(f => f.ProjectId == projectId && f.RelativePath == relativePath, cancellationToken);
  }

  /// <summary>
  /// Gets or creates a FileSystemNode, recursively creating parent directories as needed.
  /// </summary>
  public async Task<FileSystemNode> GetOrCreateAsync(
      int projectId,
      string relativePath,
      bool isDirectory,
      long? fileSizeBytes = null,
      CancellationToken cancellationToken = default) {

    var normalizedPath = relativePath.Replace('\\', '/').Trim('/');

    // Check if already exists (single query)
    var existing = await _context.FileSystemNodes
        .AsNoTracking()
        .FirstOrDefaultAsync(
            n => n.ProjectId == projectId && n.RelativePath == normalizedPath,
            cancellationToken);

    if (existing != null) {
      // Update metadata if it's a file and size changed
      if (!isDirectory && fileSizeBytes.HasValue && existing.SizeInBytes != fileSizeBytes.Value) {
        // Re-attach and update
        var tracked = await _context.FileSystemNodes
            .FirstOrDefaultAsync(
                n => n.Id == existing.Id,
                cancellationToken);

        if (tracked != null) {
          tracked.UpdateMetadata(fileSizeBytes.Value, DateTime.UtcNow);
          await _context.SaveChangesAsync(cancellationToken);
        }
      }
      return existing;
    }

    // Parse the path components    
    var fileName = Path.GetFileName(normalizedPath);
    var extension = isDirectory ? null : Path.GetExtension(normalizedPath);

    // Get parent directory path
    var parentPath = Path.GetDirectoryName(normalizedPath)?.Replace('\\', '/');
    int? parentId = null;

    // Recursively ensure parent directory exists
    if (!string.IsNullOrEmpty(parentPath)) {
      var parent = await GetOrCreateAsync(
          projectId,
          parentPath,
          isDirectory: true, // parent is always a directory
          cancellationToken: cancellationToken);
      parentId = parent.Id;
    }

    // Create the node
    FileSystemNode node;
    if (isDirectory) {
      node = FileSystemNode.CreateDirectory(projectId, fileName, normalizedPath, parentId);
    } else {
      node = FileSystemNode.CreateFile(
          projectId,
          fileName,
          normalizedPath,
          fileSizeBytes ?? 0,
          extension ?? string.Empty,
          parentId);
    }

    _context.FileSystemNodes.Add(node);
    await _context.SaveChangesAsync(cancellationToken);

    _logger.LogDebug("Created FileSystemNode: {Path} (IsDirectory: {IsDirectory}, ParentId: {ParentId})",
        normalizedPath, isDirectory, parentId);

    return node;
  }

  public async Task<List<FileSystemNode>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default) {
    return await _context.FileSystemNodes
        .Where(f => f.ProjectId == projectId)
        .OrderBy(f => f.RelativePath)
        .ToListAsync(cancellationToken);
  }

  public async Task<List<FileSystemNode>> GetChildrenAsync(int projectId, int? parentId, CancellationToken cancellationToken = default) {
    return await _context.FileSystemNodes
        .Where(f => f.ProjectId == projectId && f.ParentId == parentId)
        .OrderBy(f => f.IsDirectory).ThenBy(f => f.Name)
        .ToListAsync(cancellationToken);
  }

  public async Task<List<FileSystemNode>> GetTreeAsync(int projectId, int? parentId, int maxDepth = 10, CancellationToken cancellationToken = default) {
    if (maxDepth <= 0) return new List<FileSystemNode>();

    var nodes = await GetChildrenAsync(projectId, parentId, cancellationToken);

    if (maxDepth > 1) {
      foreach (var node in nodes.Where(n => n.IsDirectory)) {
        var children = await GetTreeAsync(projectId, node.Id, maxDepth - 1, cancellationToken);
        // Note: This loads into memory. For EF Core tracking, you might need a different approach
      }
    }

    return nodes;
  }

  public async Task<bool> ExistsAsync(int projectId, string relativePath, CancellationToken cancellationToken = default) {
    return await _context.FileSystemNodes
        .AnyAsync(f => f.ProjectId == projectId && f.RelativePath == relativePath, cancellationToken);
  }

  public async Task<FileSystemNode> AddAsync(FileSystemNode node, CancellationToken cancellationToken = default) {
    await _context.FileSystemNodes.AddAsync(node, cancellationToken);
    return node;
  }

  public async Task AddRangeAsync(IEnumerable<FileSystemNode> nodes, CancellationToken cancellationToken = default) {
    await _context.FileSystemNodes.AddRangeAsync(nodes, cancellationToken);
  }

  public Task UpdateAsync(FileSystemNode node, CancellationToken cancellationToken = default) {
    _context.FileSystemNodes.Update(node);
    return Task.CompletedTask;
  }

  public Task DeleteAsync(FileSystemNode node, CancellationToken cancellationToken = default) {
    _context.FileSystemNodes.Remove(node);
    return Task.CompletedTask;
  }

  public async Task DeleteByProjectIdAsync(int projectId, CancellationToken cancellationToken = default) {
    var nodes = await _context.FileSystemNodes
        .Where(f => f.ProjectId == projectId)
        .ToListAsync(cancellationToken);

    _context.FileSystemNodes.RemoveRange(nodes);
  }

  public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
    return await _context.SaveChangesAsync(cancellationToken);
  }

  public IQueryable<FileSystemNode> GetQueryable() {
    return _context.FileSystemNodes.AsQueryable();
  }
}
