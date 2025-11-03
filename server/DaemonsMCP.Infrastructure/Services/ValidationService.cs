using DaemonsMCP.Domain.Constants;
using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DaemonsMCP.Infrastructure.Services {
  public class ValidationService : IValidationService {
    private readonly ISettingRepository _settingRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ILogger<IValidationService> _logger;
    public ValidationService( ISettingRepository settingRepository, IProjectRepository projectRepository, ILogger<IValidationService> logger ) {
        _settingRepository = settingRepository;
        _projectRepository = projectRepository;
        _logger = logger;
    }

    public async Task<FileValidationContext> ValidateAndPrepareFolder(int projectId, string relativePath, bool isNewFolder) {
      var project = await _projectRepository.GetByIdAsync(projectId);
      if (project == null) {
        throw new ValidationException($"Project with ID {projectId} does not exist.");
      }
      var fullPath = string.IsNullOrEmpty(relativePath) ? project.RootPath : Path.Combine(project.RootPath, relativePath);
      fullPath = Path.GetFullPath(fullPath);
      var normalizedProjectPath = Path.GetFullPath(project.RootPath);
      if (!fullPath.StartsWith(normalizedProjectPath, StringComparison.OrdinalIgnoreCase))
        throw new UnauthorizedAccessException("Path must be within project boundaries");
      if (!isNewFolder && !Directory.Exists(fullPath)) {
        throw new DirectoryNotFoundException($"Directory not found: {fullPath}");
      }
      var filters = await GetFileSystemFiltersAsync();
      if (!IsFolderAllowed(fullPath, filters)) {
        throw new UnauthorizedAccessException("Access to this folder blocked via settings.");
      }      
      var validationContext = new FileValidationContext(){
        Project = project,
        FullPath = fullPath,
        RelativePath = Path.GetRelativePath(project.RootPath, fullPath),
        Filters = filters
      };
      return validationContext;
    }

    private bool IsFolderAllowed(string filePath, FileSystemFilters filters) {                  
      var directory = filePath;
      // Check if path contains blocked directories
      if (directory != null && filters.BlockedFolders.Any(blocked =>
          directory.Contains(blocked, StringComparison.OrdinalIgnoreCase))) return false;     
      return true;
    }


    public async Task<FileValidationContext> ValidateAndPrepareFile(int projectId, string relativePath, bool isNewFile) {
      var project = await _projectRepository.GetByIdAsync(projectId);
      if (project == null) {
        throw new ValidationException($"Project with ID {projectId} does not exist.");
      }

      var fullPath = string.IsNullOrEmpty(relativePath) ? project.RootPath : Path.Combine(project.RootPath, relativePath);
      fullPath = Path.GetFullPath(fullPath);
      var normalizedProjectPath = Path.GetFullPath(project.RootPath);

      if (!fullPath.StartsWith(normalizedProjectPath, StringComparison.OrdinalIgnoreCase))
        throw new UnauthorizedAccessException("Path must be within project boundaries");

      if (!isNewFile && !File.Exists(fullPath)) {
        throw new FileNotFoundException($"File not found: {fullPath}");
      }

      var filters = await GetFileSystemFiltersAsync();
      if (! IsFileAllowed(fullPath, filters)) {
        throw new UnauthorizedAccessException("GetFileItemById Access to this file type is not allowed for security reasons.");
      }     
      
      var validationContext = new FileValidationContext(){
        Project = project,
        FullPath = fullPath,
        RelativePath = Path.GetRelativePath(project.RootPath, fullPath),
        Filters = filters
      };
      return validationContext;
    }

    private bool IsFileAllowed(string filePath, FileSystemFilters filters) {


      var fileName = Path.GetFileName(filePath);
      var extension = Path.GetExtension(filePath);
      var directory = Path.GetDirectoryName(filePath);

      // Check blocked extensions first (takes precedence)
      if (filters.BlockedExtensions.Any() && filters.BlockedExtensions.Contains(extension)) {
        return false;
      }

      if (filters.BlockedFiles.Any() && filters.BlockedFiles.Contains(fileName)) {
        return false;
      }

      // Check if path contains blocked directories
      if (directory != null && filters.BlockedFolders.Any(blocked =>
          directory.Contains(blocked, StringComparison.OrdinalIgnoreCase))) return false;

      // If no allowed list specified, allow anything not blocked
      if (!filters.AllowedExtensions.Any()) {
        return true;
      }

      // Check allowed list
      return filters.AllowedExtensions.Contains(extension);

    }

    public void ValidatePath(string path) {
      if (string.IsNullOrEmpty(path))
        throw new ArgumentException("Path is required", nameof(path));

      // Basic path safety checks
      if (path.Contains(".."))
        throw new ArgumentException("Path traversal not allowed", nameof(path));

      if (path.Contains("~/"))
        throw new ArgumentException("Home directory references not allowed", nameof(path));
    }

    public void ValidateContent(string content) {
      if (content == null)
        throw new ArgumentException("Content cannot be null", nameof(content));
    }

    public async Task<FileSystemFilters> GetFileSystemFiltersAsync(CancellationToken cancellationToken = default) {
      var settings = await _settingRepository.GetAllAsDictionaryAsync(cancellationToken);
      
      var blockedFoldersSetting = settings.GetValueOrDefault("FileSystem.BlockedFolders", "");
      var blockedExtensionsSetting = settings.GetValueOrDefault("FileSystem.BlockedExtensions", "");
      var allowedExtensionsSetting = settings.GetValueOrDefault("FileSystem.AllowedExtensions", "");
      var blockedFilesSetting = settings.GetValueOrDefault("FileSystem.BlockedFiles", "");

      var blockedFolders = new HashSet<string>(
        (blockedFoldersSetting ?? string.Empty)
        .Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(f => f.Trim()), 
        StringComparer.OrdinalIgnoreCase);

      var blockedExtensions = new HashSet<string>(
        (blockedExtensionsSetting ?? string.Empty)
        .Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(ext => ext.StartsWith(".") ? ext.Trim() : "." + ext.Trim()), 
        StringComparer.OrdinalIgnoreCase);

      var allowedExtensions = new HashSet<string>(
        (allowedExtensionsSetting ?? string.Empty)
        .Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(ext => ext.StartsWith(".") ? ext.Trim() : "." + ext.Trim()), 
        StringComparer.OrdinalIgnoreCase);

      var blockedFiles = new HashSet<string>(
        (blockedFilesSetting ?? string.Empty)
        .Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(f => f.Trim()), 
        StringComparer.OrdinalIgnoreCase);

      return new FileSystemFilters(
        blockedFolders,
        blockedExtensions,
        allowedExtensions,
        blockedFiles
      );
    }

    public bool IsWriteAllowed(string filePath, FileSystemFilters filters) {      

      // Check if the file itself is allowed (same rules as reading)
      if (!IsFileAllowed(filePath, filters)) {
        _logger.LogDebug($"Setting IsFileAllowed false {filePath}");
        return false;
      }

      // Additional write-specific security checks
      return IsWritePathSafe(filePath);
    }

    private bool IsWritePathSafe(string filePath) {
      try {
        
        if (filePath.Contains("..") || filePath.Contains("~/")) {
          _logger.LogDebug($"Directory traversal attempt detected: {filePath}");
          return false;
        }
        
        var suspiciousPatterns = new[] { "%", "$", "`" };
        if (suspiciousPatterns.Any(pattern => filePath.Contains(pattern))) {
          _logger.LogDebug($"Suspicious pattern detected in path: {filePath}");
          return false;
        }
        
        var checkPath = Path.GetFullPath(filePath);        

        return true;
      } catch (Exception e) {        
        _logger.LogDebug($"caught exception {filePath} {e.Message}");
        return false;
      }
    }

  }

 
}
