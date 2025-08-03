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
      }
}
