using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP
{
    public static class SecurityFilters
    {
        private static readonly HashSet<string> BlockedFiles = new(StringComparer.OrdinalIgnoreCase)
        {
            ".env", ".env.local", ".env.production", ".env.development",
            "appsettings.json", "appsettings.Development.json", "appsettings.Production.json",
            "web.config", "app.config",
            "secrets.json", "user-secrets.json",
            ".gitignore", // might contain sensitive paths
            "id_rsa", "id_ed25519", ".pem", ".key", ".p12", ".pfx",
            "database.db", ".sqlite", ".db"
        };

        private static readonly HashSet<string> BlockedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".key", ".pem", ".p12", ".pfx", ".crt", ".cer",
            ".db", ".sqlite", ".sqlite3",
            ".log" // might contain sensitive info
        };

        private static readonly HashSet<string> BlockedDirectories = new(StringComparer.OrdinalIgnoreCase)
        {
            ".git", ".vs", "bin", "obj", "node_modules",
            "logs", "temp", ".ssh"
        };

        public static bool IsFileAllowed(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var extension = Path.GetExtension(filePath);
            var directory = Path.GetDirectoryName(filePath);
            if (!IsFileSizeAllowed(filePath)) {            
                return false;
            }


            if (GlobalConfig.IsConfigured) {
              var security = GlobalConfig.Security;

              // Check blocked extensions first (takes precedence)
              if (security.BlockedExtensions.Any() && security.BlockedExtensions.Contains(extension)) {
                return false;
              }

              // If no allowed list specified, allow anything not blocked
              if (!security.AllowedExtensions.Any()) {
                return true;
              }

              // Check allowed list
              return security.AllowedExtensions.Contains(extension);
            }

            // Check blocked files
            if (BlockedFiles.Contains(fileName)) return false;
        
            // Check blocked extensions
            if (BlockedExtensions.Contains(extension)) return false;
        
            // Check if path contains blocked directories
            if (directory != null && BlockedDirectories.Any(blocked => 
                directory.Contains(blocked, StringComparison.OrdinalIgnoreCase))) return false;

            return true;            

            
        }

        private static long? _maxFileSizeBytes;

        public static bool IsFileSizeAllowed(string filePath) {
          try {
            var fileInfo = new FileInfo(filePath);
            return IsFileSizeAllowed(fileInfo.Length);
          } catch {
            // If we can't get file size, assume it's too large
            return false;
          }
        }

        public static bool IsFileSizeAllowed(long fileSizeBytes) {
          var maxSizeBytes = GetMaxFileSizeBytes();
          return fileSizeBytes <= maxSizeBytes;
        }

        private static long GetMaxFileSizeBytes() {
          if (_maxFileSizeBytes.HasValue)
            return _maxFileSizeBytes.Value;

          try {
            _maxFileSizeBytes = FileSizeHelper.ParseFileSize(GlobalConfig.Security.MaxFileSize);
          } catch (Exception ex) {
            Console.Error.WriteLine($"[DaemonsMCP][Security] Invalid MaxFileSize format '{GlobalConfig.Security.MaxFileSize}': {ex.Message}. Using default 10MB.");
            _maxFileSizeBytes = 10 * 1024 * 1024; // 10MB default
          }

          return _maxFileSizeBytes.Value;
        }

        /// <summary>
        /// Reset cached values (useful for testing or config reloading)
        /// </summary>
        public static void ResetCache() {
          _maxFileSizeBytes = null;
        }


    /// <summary>
    /// Determines if write operations are allowed for the specified file path.
    /// This includes creating new files and updating existing files.
    /// </summary>
    /// <param name="filePath">The full path to the file</param>
    /// <returns>True if write operations are allowed, false otherwise</returns>
    public static bool IsWriteAllowed(string filePath) {
      // First check if writes are globally enabled
      if (GlobalConfig.IsConfigured && !GlobalConfig.Security.AllowWrite) {
        return false;
      }

      // Check if the path is write-protected
      if (IsPathWriteProtected(filePath)) {
        return false;
      }

      // Check if the file itself is allowed (same rules as reading)
      if (!IsFileAllowed(filePath)) {
        return false;
      }

      // Check write-specific file size limits
      if (!IsWriteFileSizeAllowed(filePath)) {
        return false;
      }

      // Additional write-specific security checks
      return IsWritePathSafe(filePath);
    }

    /// <summary>
    /// Determines if delete operations are allowed for the specified file path.
    /// More restrictive than write operations due to irreversible nature.
    /// </summary>
    /// <param name="filePath">The full path to the file</param>
    /// <returns>True if delete operations are allowed, false otherwise</returns>
    public static bool IsDeleteAllowed(string filePath) {
      // Delete requires write permissions to be enabled
      if (!IsWriteAllowed(filePath)) {
        return false;
      }

      var fileName = Path.GetFileName(filePath);
      var extension = Path.GetExtension(filePath);

      // Additional restrictions for delete operations
      var criticalFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
      {
                "Program.cs", "Startup.cs", "Main.cs",
                ".csproj", ".sln", ".gitignore", "README.md",
                "package.json", "requirements.txt", "Dockerfile"
            };

      var criticalExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
      {
                ".sln", ".csproj", ".vbproj", ".fsproj"
            };

      // Don't allow deletion of critical project files
      if (criticalFiles.Contains(fileName) || criticalExtensions.Contains(extension)) {
        return false;
      }

      // Don't allow deletion of files in critical directories
      var criticalDirectories = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
      {
                ".git", ".vs", "Properties"
            };

      var directory = Path.GetDirectoryName(filePath);
      if (directory != null && criticalDirectories.Any(critical =>
          directory.Contains(critical, StringComparison.OrdinalIgnoreCase))) {
        return false;
      }

      return true;
    }

    /// <summary>
    /// Checks if a path is in the write-protected paths list.
    /// </summary>
    /// <param name="filePath">The full path to check</param>
    /// <returns>True if the path is write-protected, false otherwise</returns>
    public static bool IsPathWriteProtected(string filePath) {
      if (!GlobalConfig.IsConfigured) {
        // Use default protected paths when no config
        return BlockedDirectories.Any(blocked =>
            filePath.Contains(blocked, StringComparison.OrdinalIgnoreCase));
      }

      var security = GlobalConfig.Security;
      var normalizedPath = Path.GetFullPath(filePath).Replace('\\', '/');

      return security.WriteProtectedPaths.Any(protectedPath =>
      {
        var normalizedProtected = protectedPath.Replace('\\', '/');

        // Check if the file path contains the protected path
        return normalizedPath.Contains(normalizedProtected, StringComparison.OrdinalIgnoreCase) ||
               // Check if it's a direct match for directory protection
               normalizedPath.StartsWith(normalizedProtected.TrimEnd('/') + "/", StringComparison.OrdinalIgnoreCase);
      });
    }

    /// <summary>
    /// Validates that the file path is safe for write operations (prevents directory traversal attacks).
    /// </summary>
    /// <param name="filePath">The file path to validate</param>
    /// <returns>True if the path is safe, false otherwise</returns>
    private static bool IsWritePathSafe(string filePath) {
      try {
        // Check for directory traversal attempts
        if (filePath.Contains("..") || filePath.Contains("~/")) {
          return false;
        }

        // Check for absolute paths that might escape project boundaries
        if (Path.IsPathRooted(filePath)) {
          // Allow only if it's within a configured project path
          return GlobalConfig.Projects.Values.Any(project =>
              filePath.StartsWith(project.Path, StringComparison.OrdinalIgnoreCase));
        }

        // Additional checks for suspicious patterns
        var suspiciousPatterns = new[] { "\\\\", "//", "%", "$", "`" };
        if (suspiciousPatterns.Any(pattern => filePath.Contains(pattern))) {
          return false;
        }

        // Ensure the path can be properly normalized
        _ = Path.GetFullPath(filePath);

        return true;
      } catch (Exception) {
        // If path normalization fails, it's not safe
        return false;
      }
    }

    /// <summary>
    /// Checks if the file size is within write operation limits.
    /// </summary>
    /// <param name="filePath">The file path (used to get current size if file exists)</param>
    /// <param name="contentSize">Optional: size of content to be written</param>
    /// <returns>True if size is allowed, false otherwise</returns>
    private static bool IsWriteFileSizeAllowed(string filePath, long? contentSize = null) {
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
    /// Public method to check if content size is allowed for write operations.
    /// </summary>
    /// <param name="contentSize">Size of content in bytes</param>
    /// <returns>True if size is allowed, false otherwise</returns>
    public static bool IsWriteContentSizeAllowed(long contentSize) {
      return contentSize <= GetMaxWriteFileSizeBytes();
    }

    /// <summary>
    /// Gets the maximum allowed file size for write operations.
    /// </summary>
    /// <returns>Maximum file size in bytes</returns>
    private static long GetMaxWriteFileSizeBytes() {
      if (!GlobalConfig.IsConfigured) {
        return 5 * 1024 * 1024; // 5MB default
      }

      try {
        return FileSizeHelper.ParseFileSize(GlobalConfig.Security.MaxFileWriteSize);
      } catch {
        return 5 * 1024 * 1024; // 5MB fallback
      }
    }

    // Existing methods (IsFileSizeAllowed, GetMaxFileSizeBytes, ResetCache) remain unchanged
    private static long? _maxWriteFileSizeBytes;

  }
}
