using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaemonsMCP.Core.Config;
using DaemonsMCP.Core.Extensions;
using DaemonsMCP.Core.Models;
using Microsoft.Extensions.Logging;

namespace DaemonsMCP.Core.Services {
  public class SecurityService : ISecurityService {
    private readonly IAppConfig _config;
    private readonly SecuritySettings _settings;
    private readonly ILogger<SecurityService> _logger;
    public SecurityService(ILoggerFactory loggerFactory, IAppConfig config) {
      _logger = loggerFactory.CreateLogger<SecurityService>() ?? throw new ArgumentNullException(nameof(loggerFactory));
      _config = config ?? throw new ArgumentNullException(nameof(config));
      _settings = _config.Security ?? throw new ArgumentNullException(nameof(_config.Security));
    }

    private long _maxFileSizeBytes = -1;
    private long _maxFileWriteSize = -1;
  
    public bool IsContentSizeAllowed(long contentSize) {
      if (contentSize < 0) throw new ArgumentOutOfRangeException(nameof(contentSize), "Content size cannot be negative.");
      if (_maxFileSizeBytes == -1) { 
        _maxFileSizeBytes = GetMaxFileSizeBytes();
      }
      return contentSize <= _maxFileWriteSize;      
    }
    private long GetMaxFileSizeBytes() {
      if (_maxFileSizeBytes != -1)
        return _maxFileSizeBytes;

      try {
        _maxFileSizeBytes = _config.Security.MaxFileSize.ParseFileSize();
      } catch (Exception ex) {
        _logger.LogError($"{Cx.Dd2} Invalid MaxFileSize format '{_config.Security.MaxFileSize}': {ex.Message}. Using default 10MB.");
        _maxFileSizeBytes = 10 * 1024 * 1024; // 10MB default
      }

      return _maxFileSizeBytes;
    }

    /// <summary>
    /// Gets the maximum allowed file size for write operations.
    /// </summary>
    /// <returns>Maximum file size in bytes</returns>
    private long GetMaxWriteFileSizeBytes() {
      if (!_config.IsConfigured) {
        return 5 * 1024 * 1024; // 5MB default
      }
      if (_maxFileWriteSize == -1) {
        _maxFileWriteSize = _settings.MaxFileWriteSize.ParseFileSize();
      }
      try {
        return _maxFileWriteSize;
      } catch {
        return 5 * 1024 * 1024; // 5MB fallback
      }
    }

    public bool IsDeleteAllowed(string filePath) {
      // DeleteClassItem requires write permissions to be enabled
      if (!IsWriteAllowed(filePath)) {
        return false;
      }

      var fileName = Path.GetFileName(filePath);
      var extension = Path.GetExtension(filePath);
      var directory = Path.GetDirectoryName(filePath);

      // Additional restrictions for delete operations
      var criticalFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {        
        ".csproj", ".sln", ".gitignore", "package.json", "Dockerfile"
      };

      var criticalExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
        ".sln", ".csproj", ".vbproj", ".fsproj"
      };

      // Don't allow deletion of critical project files
      if (criticalFiles.Contains(fileName) || criticalExtensions.Contains(extension)) {
        return false;
      }

      // Don't allow deletion of files in critical directories
      var criticalDirectories = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
      { ".git", ".vs", "Properties"
      };
      
      if (directory != null && criticalDirectories.Any(critical =>
          directory.Contains(critical, StringComparison.OrdinalIgnoreCase))) {
        return false;
      }

      return true;
    }

    public bool IsFileAllowed(string filePath) {

      var fileName = Path.GetFileName(filePath);
      var extension = Path.GetExtension(filePath);
      var directory = Path.GetDirectoryName(filePath);

      if (!IsFileSizeAllowed(filePath)) {
        return false;
      }
           
      var security = _config.Security;

      // Check blocked extensions first (takes precedence)
      if (security.BlockedExtensions.Any() && security.BlockedExtensions.Contains(extension)) {
        return false;
      }

      if (security.BlockedFileNames.Any() && security.BlockedFileNames.Contains(fileName)) { 
        return false;
      }

      // Check if path contains blocked directories
      if (directory != null && security.WriteProtectedPaths.Any(blocked =>
          directory.Contains(blocked, StringComparison.OrdinalIgnoreCase))) return false;

      // If no allowed list specified, allow anything not blocked
      if (!security.AllowedExtensions.Any()) {
        return true;
      }

      // Check allowed list
      return security.AllowedExtensions.Contains(extension);
      
    }

    public bool IsPathWriteProtected(string filePath) {      

      var security = _config.Security;      
      var normalizedPath = Path.GetFullPath(filePath);
      var directory = Path.GetDirectoryName(filePath);
      if (directory == null) directory = normalizedPath;

      return security.WriteProtectedPaths.Any(protectedPath => {
        // Check if the file path contains the protected path
        return directory.Contains(protectedPath, StringComparison.OrdinalIgnoreCase) ||
               // Check if it's a direct match for directory protection
               directory.StartsWith(protectedPath.TrimEnd('/') + "/", StringComparison.OrdinalIgnoreCase);
      });
    }

    public bool IsWriteAllowed(string filePath) {
      // First check if writes are globally enabled
      if (!_settings.AllowWrite) {
        _logger.LogDebug($"{Cx.Dd2} Setting AllowWrite says no");
        return false;
      }

      // Check if the path is write-protected
      if (IsPathWriteProtected(filePath)) {
        _logger.LogDebug($"{Cx.Dd2} Setting IsPathWriteProtected true {filePath}");
        return false;
      }

      // Check if the file itself is allowed (same rules as reading)
      if (!IsFileAllowed(filePath)) {
        if (Cx.IsDebug) _logger.LogDebug($"{Cx.Dd2} Setting IsFileAllowed false {filePath}");
        return false;
      }

      // Check write-specific file size limits (if file exists create will check later)
      if (File.Exists(filePath)) {
        if (!IsWriteFileSizeAllowed(filePath)) {
          if (Cx.IsDebug) _logger.LogDebug($"{Cx.Dd2} Setting IsWriteFileSizeAllowed false {filePath}");
          return false;
        }
      }

      // Additional write-specific security checks
      return IsWritePathSafe(filePath);
    }

    public bool IsWriteContentSizeAllowed(long contentSize) {
      return contentSize <= GetMaxWriteFileSizeBytes();
    }

    public bool IsFileSizeAllowed(string filePath) {
      try {
        if (!File.Exists(filePath)) {          
          return true;
        }
        var fileInfo = new FileInfo(filePath);
        return IsFileSizeAllowed(fileInfo.Length);
      } catch (Exception e) {
        // If we can't get file size, assume it's too large
        _logger.LogDebug($"[DaemonsMCP][Security] File Size lookup excepts {e.Message}");
        return false;
      }
    }

    public bool IsFileSizeAllowed(long fileSizeBytes) {
      var maxSizeBytes = GetMaxFileSizeBytes();
      return fileSizeBytes <= maxSizeBytes;
    }

   
    /// <summary>
    /// Checks if the file size is within write operation limits.
    /// </summary>
    /// <param name="filePath">The file path (used to get current size if file exists)</param>
    /// <param name="contentSize">Optional: size of content to be written</param>
    /// <returns>True if size is allowed, false otherwise</returns>
    private bool IsWriteFileSizeAllowed(string filePath, long? contentSize = null) {
      var maxWriteSize = GetMaxWriteFileSizeBytes();

      // If we have content size, check that
      if (contentSize.HasValue) {
        return contentSize.Value <= maxWriteSize;
      }

      // If file exists, check current size
      if (File.Exists(filePath)) {
        try {
          var fileInfo = new FileInfo(filePath);
          return fileInfo.Length <= maxWriteSize;
        } catch {
          return false;
        }
      }

      // For new files, assume it's okay (will be checked when content is provided)
      return true;
    }

    /// <summary>
    /// Validates that the file path is safe for write operations (prevents directory traversal attacks).
    /// </summary>
    /// <param name="filePath">The file path to validate</param>
    /// <returns>True if the path is safe, false otherwise</returns>
    private bool IsWritePathSafe(string filePath) {
      try {
        // Check for directory traversal attempts
        if (filePath.Contains("..") || filePath.Contains("~/")) {
          _logger.LogDebug($"{Cx.Dd2} Directory traversal attempt detected: {filePath}");
          return false;
        }

        // Additional checks for suspicious patterns
        var suspiciousPatterns = new[] { "%", "$", "`" };
        if (suspiciousPatterns.Any(pattern => filePath.Contains(pattern))) {
          _logger.LogDebug($"{Cx.Dd2} Suspicious pattern detected in path: {filePath}");
          return false;
        }

        // Ensure the path can be properly normalized
        var checkPath = Path.GetFullPath(filePath);

        // Check for absolute paths that might escape project boundaries
        if (Path.IsPathRooted(filePath)) {
          // Allow only if it's within a configured project path
          var boundryCheck = _config.Projects.Values.Any(project =>
              filePath.StartsWith(project.Path, StringComparison.OrdinalIgnoreCase));
          if (!boundryCheck) { 
            _logger.LogDebug($"final rooted boundy check false"); 
          }
          return boundryCheck;
        }

        return true;
      } catch (Exception e) {
        // If path normalization fails, it's not safe
        _logger.LogDebug($"[DaemonsMCP][Security] caught exception {filePath} {e.Message}");
        return false;
      }
    }
  }
}
