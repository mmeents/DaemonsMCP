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

            // Check blocked files
            if (BlockedFiles.Contains(fileName)) return false;
        
            // Check blocked extensions
            if (BlockedExtensions.Contains(extension)) return false;
        
            // Check if path contains blocked directories
            if (directory != null && BlockedDirectories.Any(blocked => 
                directory.Contains(blocked, StringComparison.OrdinalIgnoreCase))) return false;

            return true;
        }
    }
}
