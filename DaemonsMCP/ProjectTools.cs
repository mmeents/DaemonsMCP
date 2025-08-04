using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCPSharp;

namespace DaemonsMCP {
  /// <summary>
  /// Project management tools for DaemonsMCP
  /// </summary>
  public class ProjectTools {
    /// <summary>
    /// Gets list of available projects. A project is the name of the root folder.
    /// </summary>
    /// <returns>List of projects with names and descriptions</returns>
    [McpTool("local-list-projects")]
    public static object ListProjects() {
      return System.Text.Json.JsonSerializer.Serialize(new {projects = GlobalConfig.Projects.Values.ToArray() });      
    }

    /// <summary>
    /// Gets list of directories in the project. The project name is required and must match the current project.
    /// </summary>
    /// <param name="projectName">Project name from list-projects</param>
    /// <param name="path">Path to the directory or file. If empty, the root of the project is used.</param>
    /// <param name="filter">Filter for files or directories. If empty, no filter is applied.</param>
    /// <returns>List of directories</returns>
    [McpTool("local-list-project-directories")]
    public static object ListProjectDirectories(
        [Description("Project name from list-projects")] string projectName,
        [Description("Path to the directory or file. If empty, the root of the project is used.")] string? path = null,
        [Description("Filter for files or directories. If empty, no filter is applied.")] string? filter = null) {
      if (!GlobalConfig.Projects.TryGetValue(projectName, out Project? project)) {
        throw new ArgumentException($"Invalid project name: {projectName}");
      }

      var fullPath = string.IsNullOrEmpty(path) ? project.Path : Path.Combine(project.Path, path);

      var searchPattern = string.IsNullOrEmpty(filter) ? "*" : filter;
      var directories = Directory.GetDirectories(fullPath, searchPattern, SearchOption.TopDirectoryOnly);

      var relativeDirs = directories.Select(dir =>
          dir.Substring(project.Path.Length).TrimStart(Path.DirectorySeparatorChar)
      ).ToArray();

      return System.Text.Json.JsonSerializer.Serialize(new { directories = relativeDirs });
    }

    /// <summary>
    /// Gets list of files in the project. The project name is required and must match the current project.
    /// </summary>
    /// <param name="projectName">Project name from list-projects</param>
    /// <param name="path">Path to the directory or file. If empty, the root of the project is used.</param>
    /// <param name="filter">Filter for files or directories. If empty, no filter is applied.</param>
    /// <returns>List of files</returns>
    [McpTool("local-list-project-files")]
    public static object ListProjectFiles(
        [Description("Project name from list-projects")] string projectName,
        [Description("Path to the directory or file. If empty, the root of the project is used.")] string? path = null,
        [Description("Filter for files or directories. If empty, no filter is applied.")] string? filter = null) {
      if (!GlobalConfig.Projects.TryGetValue(projectName, out Project? project)) {
        throw new ArgumentException($"Invalid project name: {projectName}");
      }

      var fullPath = string.IsNullOrEmpty(path) ? project.Path : Path.Combine(project.Path, path);
      var searchPattern = string.IsNullOrEmpty(filter) ? "*" : filter;

      var files = Directory.GetFiles(fullPath, searchPattern, SearchOption.TopDirectoryOnly)
          .Where(file => SecurityFilters.IsFileAllowed(file))
          .Select(file => file.Substring(project.Path.Length).TrimStart(Path.DirectorySeparatorChar))
          .ToArray();

      return System.Text.Json.JsonSerializer.Serialize(new { files = files });
    }

    /// <summary>
    /// Gets the content of a file in the project. The project name is required and must match the current project. The path to the file is also required.
    /// </summary>
    /// <param name="projectName">Project name from list-projects</param>
    /// <param name="path">Path to the file</param>
    /// <returns>File content and metadata</returns>
    [McpTool("local-get-project-file")]
    public static async Task<object> GetProjectFile(
        [Description("Project name from list-projects")] string projectName,
        [Description("Path to the file")] string path) {
      if (!GlobalConfig.Projects.TryGetValue(projectName, out Project? project)) {
        throw new ArgumentException($"Invalid project name: {projectName}");
      }

      if (string.IsNullOrEmpty(path)) {
        throw new ArgumentException("Path is required");
      }

      var fullFilePath = Path.Combine(project.Path, path);

      if (!File.Exists(fullFilePath)) {
        throw new FileNotFoundException($"File not found: {fullFilePath}");
      }

      if (!SecurityFilters.IsFileAllowed(fullFilePath)) {
        throw new UnauthorizedAccessException("Access to this file type is not allowed for security reasons.");
      }

      var fileInfo = new FileInfo(fullFilePath);
      var relativePath = fileInfo.FullName.Substring(project.Path.Length).TrimStart(Path.DirectorySeparatorChar);
      var fileExtension = fileInfo.Extension.ToLowerInvariant();
      var fileEncoding = MimeTypesMap.DetectFileEncoding(fullFilePath);
      string contentType = MimeTypesMap.GetMimeType(fileExtension);
      bool isBinary = MimeTypesMap.IsBinaryFile(fullFilePath);

      var fileContent = "";
      if (!isBinary) {
        fileContent = await File.ReadAllTextAsync(fullFilePath, System.Text.Encoding.UTF8);
      }

      return new {
        fileName = fileInfo.Name,
        path = relativePath,
        size = fileInfo.Length,
        contentType,
        encoding = fileEncoding,
        content = fileContent
      };
    }

    /// <summary>
    /// Creates a new file in the project with the specified content. 
    /// SAFETY: Requires write permissions enabled and validates all security rules.
    /// </summary>
    /// <param name="projectName">Project name from list-projects</param>
    /// <param name="path">Path to the new file (relative to project root)</param>
    /// <param name="content">File content to write</param>
    /// <param name="createDirectories">Create parent directories if they don't exist</param>
    /// <param name="overwrite">Allow overwriting existing files (default: false for safety)</param>
    /// <returns>Operation result with file information</returns>
    [McpTool("local-create-project-file")]
    public static async Task<object> CreateProjectFile(
        [Description("Project name from list-projects")] string projectName,
        [Description("Path to the new file (relative to project root)")] string path,
        [Description("File content to write")] string content,
        [Description("Create parent directories if they don't exist")] bool createDirectories = true,
        [Description("Allow overwriting existing files (default: false for safety)")] bool overwrite = false) {

      // Validate inputs
      if (string.IsNullOrWhiteSpace(projectName)) {
        throw new ArgumentException("Project name is required", nameof(projectName));
      }

      if (string.IsNullOrWhiteSpace(path)) {
        throw new ArgumentException("File path is required", nameof(path));
      }

      if (content == null) {
        throw new ArgumentException("Content cannot be null", nameof(content));
      }

      // Get project
      if (!GlobalConfig.Projects.TryGetValue(projectName, out Project? project)) {
        throw new ArgumentException($"Invalid project name: {projectName}");
      }

      // Build full path
      var fullFilePath = Path.Combine(project.Path, path);

      // SAFETY CHECK: Ensure path is within project boundaries
      var normalizedProjectPath = Path.GetFullPath(project.Path);
      var normalizedFilePath = Path.GetFullPath(fullFilePath);
      if (!normalizedFilePath.StartsWith(normalizedProjectPath, StringComparison.OrdinalIgnoreCase)) {
        throw new UnauthorizedAccessException("File path must be within the project directory");
      }

      // Security validations
      if (!SecurityFilters.IsWriteAllowed(fullFilePath)) {
        throw new UnauthorizedAccessException("Write operation not allowed for security reasons");
      }

      if (!SecurityFilters.IsWriteContentSizeAllowed(Encoding.UTF8.GetByteCount(content))) {
        throw new ArgumentException("Content size exceeds maximum allowed for write operations");
      }

      // Check if file already exists
      if (File.Exists(fullFilePath) && !overwrite) {
        throw new InvalidOperationException($"File already exists: {path}. Use overwrite=true to replace it.");
      }

      try {
        // Create directory if needed
        var directory = Path.GetDirectoryName(fullFilePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) {
          if (createDirectories) {
            Directory.CreateDirectory(directory);
          } else {
            throw new DirectoryNotFoundException($"Directory does not exist: {Path.GetDirectoryName(path)}");
          }
        }

        // Write the file
        await File.WriteAllTextAsync(fullFilePath, content, Encoding.UTF8);

        // Return success info
        var fileInfo = new FileInfo(fullFilePath);
        var relativePath = fileInfo.FullName.Substring(project.Path.Length).TrimStart(Path.DirectorySeparatorChar);

        return new {
          success = true,
          operation = "create",
          fileName = fileInfo.Name,
          path = relativePath,
          size = fileInfo.Length,
          created = fileInfo.CreationTime,
          message = $"File created successfully: {path}"
        };
      } catch (Exception ex) when (!(ex is UnauthorizedAccessException || ex is ArgumentException || ex is InvalidOperationException)) {
        throw new InvalidOperationException($"Failed to create file: {ex.Message}", ex);
      }
    }

    /// <summary>
    /// Updates an existing file in the project with new content.
    /// SAFETY: Creates backup before updating and validates all security rules.
    /// </summary>
    /// <param name="projectName">Project name from list-projects</param>
    /// <param name="path">Path to the file to update</param>
    /// <param name="content">New file content</param>
    /// <param name="createBackup">Create backup before update (highly recommended)</param>
    /// <param name="backupSuffix">Suffix for backup file (default: .backup.{timestamp})</param>
    /// <returns>Operation result with file information</returns>
    [McpTool("local-update-project-file")]
    public static async Task<object> UpdateProjectFile(
        [Description("Project name from list-projects")] string projectName,
        [Description("Path to the file to update")] string path,
        [Description("New file content")] string content,
        [Description("Create backup before update (highly recommended)")] bool createBackup = true,
        [Description("Suffix for backup file (default: .backup.{timestamp})")] string? backupSuffix = null) {

      // Validate inputs
      if (string.IsNullOrWhiteSpace(projectName)) {
        throw new ArgumentException("Project name is required", nameof(projectName));
      }

      if (string.IsNullOrWhiteSpace(path)) {
        throw new ArgumentException("File path is required", nameof(path));
      }

      if (content == null) {
        throw new ArgumentException("Content cannot be null", nameof(content));
      }

      // Get project
      if (!GlobalConfig.Projects.TryGetValue(projectName, out Project? project)) {
        throw new ArgumentException($"Invalid project name: {projectName}");
      }

      // Build full path
      var fullFilePath = Path.Combine(project.Path, path);

      // SAFETY CHECK: Ensure path is within project boundaries
      var normalizedProjectPath = Path.GetFullPath(project.Path);
      var normalizedFilePath = Path.GetFullPath(fullFilePath);
      if (!normalizedFilePath.StartsWith(normalizedProjectPath, StringComparison.OrdinalIgnoreCase)) {
        throw new UnauthorizedAccessException("File path must be within the project directory");
      }

      // Check if file exists
      if (!File.Exists(fullFilePath)) {
        throw new FileNotFoundException($"File not found: {path}");
      }

      // Security validations
      if (!SecurityFilters.IsWriteAllowed(fullFilePath)) {
        throw new UnauthorizedAccessException("Write operation not allowed for security reasons");
      }

      if (!SecurityFilters.IsWriteContentSizeAllowed(Encoding.UTF8.GetByteCount(content))) {
        throw new ArgumentException("Content size exceeds maximum allowed for write operations");
      }

      string? backupPath = null;

      try {
        // Create backup if requested
        if (createBackup) {
          var suffix = backupSuffix ?? $".backup.{DateTime.Now:yyyyMMdd_HHmmss}";
          backupPath = fullFilePath + suffix;
          File.Copy(fullFilePath, backupPath, true);
        }

        // Update the file
        await File.WriteAllTextAsync(fullFilePath, content, Encoding.UTF8);

        // Return success info
        var fileInfo = new FileInfo(fullFilePath);
        var relativePath = fileInfo.FullName.Substring(project.Path.Length).TrimStart(Path.DirectorySeparatorChar);
        var relativeBackupPath = backupPath != null ?
          backupPath.Substring(project.Path.Length).TrimStart(Path.DirectorySeparatorChar) : null;

        return new {
          success = true,
          operation = "update",
          fileName = fileInfo.Name,
          path = relativePath,
          size = fileInfo.Length,
          modified = fileInfo.LastWriteTime,
          backupCreated = createBackup,
          backupPath = relativeBackupPath,
          message = $"File updated successfully: {path}"
        };
      } catch (Exception ex) when (!(ex is UnauthorizedAccessException || ex is ArgumentException || ex is FileNotFoundException)) {
        // If we created a backup and the update failed, we could restore it here
        // For now, just provide informative error
        var errorMessage = $"Failed to update file: {ex.Message}";
        if (backupPath != null && File.Exists(backupPath)) {
          errorMessage += $" Backup available at: {Path.GetFileName(backupPath)}";
        }
        throw new InvalidOperationException(errorMessage, ex);
      }
    }

    /// <summary>
    /// Deletes a file from the project.
    /// SAFETY: Creates backup before deletion and has strict security validations.
    /// </summary>
    /// <param name="projectName">Project name from list-projects</param>
    /// <param name="path">Path to the file to delete</param>
    /// <param name="createBackup">Create backup before deletion (highly recommended)</param>
    /// <param name="confirmDeletion">Explicit confirmation required for safety</param>
    /// <returns>Operation result</returns>
    [McpTool("local-delete-project-file")]
    public static async Task<object> DeleteProjectFile(
        [Description("Project name from list-projects")] string projectName,
        [Description("Path to the file to delete")] string path,
        [Description("Create backup before deletion (highly recommended)")] bool createBackup = true,
        [Description("Explicit confirmation required for safety - must be true")] bool confirmDeletion = false) {

      // SAFETY: Require explicit confirmation
      if (!confirmDeletion) {
        throw new ArgumentException("Deletion requires explicit confirmation. Set confirmDeletion=true to proceed.");
      }

      // Validate inputs
      if (string.IsNullOrWhiteSpace(projectName)) {
        throw new ArgumentException("Project name is required", nameof(projectName));
      }

      if (string.IsNullOrWhiteSpace(path)) {
        throw new ArgumentException("File path is required", nameof(path));
      }

      // Get project
      if (!GlobalConfig.Projects.TryGetValue(projectName, out Project? project)) {
        throw new ArgumentException($"Invalid project name: {projectName}");
      }

      // Build full path
      var fullFilePath = Path.Combine(project.Path, path);

      // SAFETY CHECK: Ensure path is within project boundaries
      var normalizedProjectPath = Path.GetFullPath(project.Path);
      var normalizedFilePath = Path.GetFullPath(fullFilePath);
      if (!normalizedFilePath.StartsWith(normalizedProjectPath, StringComparison.OrdinalIgnoreCase)) {
        throw new UnauthorizedAccessException("File path must be within the project directory");
      }

      // Check if file exists
      if (!File.Exists(fullFilePath)) {
        throw new FileNotFoundException($"File not found: {path}");
      }

      // Security validations - delete is most restrictive
      if (!SecurityFilters.IsDeleteAllowed(fullFilePath)) {
        throw new UnauthorizedAccessException("Delete operation not allowed for security reasons");
      }

      string? backupPath = null;
      FileInfo originalFileInfo = new FileInfo(fullFilePath);

      try {
        // Create backup if requested (before deletion)
        if (createBackup) {
          var backupSuffix = $".deleted.backup.{DateTime.Now:yyyyMMdd_HHmmss}";
          backupPath = fullFilePath + backupSuffix;
          File.Copy(fullFilePath, backupPath, true);
        }

        // Delete the file
        File.Delete(fullFilePath);

        // Return success info
        var relativePath = originalFileInfo.FullName.Substring(project.Path.Length).TrimStart(Path.DirectorySeparatorChar);
        var relativeBackupPath = backupPath != null ?
          backupPath.Substring(project.Path.Length).TrimStart(Path.DirectorySeparatorChar) : null;

        return new {
          success = true,
          operation = "delete",
          fileName = originalFileInfo.Name,
          path = relativePath,
          deletedSize = originalFileInfo.Length,
          deletedAt = DateTime.Now,
          backupCreated = createBackup,
          backupPath = relativeBackupPath,
          message = $"File deleted successfully: {path}"
        };
      } catch (Exception ex) when (!(ex is UnauthorizedAccessException || ex is ArgumentException || ex is FileNotFoundException)) {
        throw new InvalidOperationException($"Failed to delete file: {ex.Message}", ex);
      }
    }

    // DIRECTORY OPERATIONS (Step 2.2)

    /// <summary>
    /// Creates a new directory in the project.
    /// SAFETY: Validates path and ensures it's within project boundaries.
    /// </summary>
    /// <param name="projectName">Project name from list-projects</param>
    /// <param name="path">Path to the new directory (relative to project root)</param>
    /// <param name="createParents">Create parent directories if they don't exist</param>
    /// <returns>Operation result</returns>
    [McpTool("local-create-project-directory")]
    public static object CreateProjectDirectory(
        [Description("Project name from list-projects")] string projectName,
        [Description("Path to the new directory (relative to project root)")] string path,
        [Description("Create parent directories if they don't exist")] bool createParents = true) {

      // Validate inputs
      if (string.IsNullOrWhiteSpace(projectName)) {
        throw new ArgumentException("Project name is required", nameof(projectName));
      }

      if (string.IsNullOrWhiteSpace(path)) {
        throw new ArgumentException("Directory path is required", nameof(path));
      }

      // Get project
      if (!GlobalConfig.Projects.TryGetValue(projectName, out Project? project)) {
        throw new ArgumentException($"Invalid project name: {projectName}");
      }

      // Build full path
      var fullDirPath = Path.Combine(project.Path, path);

      // SAFETY CHECK: Ensure path is within project boundaries
      var normalizedProjectPath = Path.GetFullPath(project.Path);
      var normalizedDirPath = Path.GetFullPath(fullDirPath);
      if (!normalizedDirPath.StartsWith(normalizedProjectPath, StringComparison.OrdinalIgnoreCase)) {
        throw new UnauthorizedAccessException("Directory path must be within the project directory");
      }

      // Check if path is write-protected
      if (SecurityFilters.IsPathWriteProtected(fullDirPath)) {
        throw new UnauthorizedAccessException("Cannot create directory in write-protected location");
      }

      // Check if directory already exists
      if (Directory.Exists(fullDirPath)) {
        throw new InvalidOperationException($"Directory already exists: {path}");
      }

      try {
        // Create directory
        if (createParents) {
          Directory.CreateDirectory(fullDirPath);
        } else {
          var parentDir = Path.GetDirectoryName(fullDirPath);
          if (!string.IsNullOrEmpty(parentDir) && !Directory.Exists(parentDir)) {
            throw new DirectoryNotFoundException($"Parent directory does not exist: {Path.GetDirectoryName(path)}");
          }
          Directory.CreateDirectory(fullDirPath);
        }

        // Return success info
        var dirInfo = new DirectoryInfo(fullDirPath);
        var relativePath = dirInfo.FullName.Substring(project.Path.Length).TrimStart(Path.DirectorySeparatorChar);

        return new {
          success = true,
          operation = "create-directory",
          directoryName = dirInfo.Name,
          path = relativePath,
          created = dirInfo.CreationTime,
          message = $"Directory created successfully: {path}"
        };
      } catch (Exception ex) when (!(ex is UnauthorizedAccessException || ex is ArgumentException || ex is InvalidOperationException)) {
        throw new InvalidOperationException($"Failed to create directory: {ex.Message}", ex);
      }
    }

    /// <summary>
    /// Deletes a directory from the project.
    /// SAFETY: Extra protections for directory deletion, requires explicit confirmation.
    /// </summary>
    /// <param name="projectName">Project name from list-projects</param>
    /// <param name="path">Path to the directory to delete</param>
    /// <param name="recursive">Delete directory and all contents (required for non-empty directories)</param>
    /// <param name="confirmDeletion">Explicit confirmation required for safety</param>
    /// <returns>Operation result</returns>
    [McpTool("local-delete-project-directory")]
    public static object DeleteProjectDirectory(
        [Description("Project name from list-projects")] string projectName,
        [Description("Path to the directory to delete")] string path,
        [Description("Delete directory and all contents (required for non-empty directories)")] bool recursive = false,
        [Description("Explicit confirmation required for safety - must be true")] bool confirmDeletion = false) {

      // SAFETY: Require explicit confirmation
      if (!confirmDeletion) {
        throw new ArgumentException("Directory deletion requires explicit confirmation. Set confirmDeletion=true to proceed.");
      }

      // Validate inputs
      if (string.IsNullOrWhiteSpace(projectName)) {
        throw new ArgumentException("Project name is required", nameof(projectName));
      }

      if (string.IsNullOrWhiteSpace(path)) {
        throw new ArgumentException("Directory path is required", nameof(path));
      }

      // Get project
      if (!GlobalConfig.Projects.TryGetValue(projectName, out Project? project)) {
        throw new ArgumentException($"Invalid project name: {projectName}");
      }

      // Build full path
      var fullDirPath = Path.Combine(project.Path, path);

      // SAFETY CHECK: Ensure path is within project boundaries
      var normalizedProjectPath = Path.GetFullPath(project.Path);
      var normalizedDirPath = Path.GetFullPath(fullDirPath);
      if (!normalizedDirPath.StartsWith(normalizedProjectPath, StringComparison.OrdinalIgnoreCase)) {
        throw new UnauthorizedAccessException("Directory path must be within the project directory");
      }

      // Check if directory exists
      if (!Directory.Exists(fullDirPath)) {
        throw new DirectoryNotFoundException($"Directory not found: {path}");
      }

      // SAFETY: Extra protection for critical directories
      var protectedDirs = new[] { ".git", ".vs", "bin", "obj", "Properties" };
      if (protectedDirs.Any(dir => path.Equals(dir, StringComparison.OrdinalIgnoreCase) ||
                                   path.StartsWith(dir + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))) {
        throw new UnauthorizedAccessException($"Cannot delete protected directory: {path}");
      }

      try {
        var dirInfo = new DirectoryInfo(fullDirPath);
        var relativePath = dirInfo.FullName.Substring(project.Path.Length).TrimStart(Path.DirectorySeparatorChar);
        var fileCount = recursive ? Directory.GetFiles(fullDirPath, "*", SearchOption.AllDirectories).Length : 0;
        var subdirCount = recursive ? Directory.GetDirectories(fullDirPath, "*", SearchOption.AllDirectories).Length : 0;

        // Delete directory
        Directory.Delete(fullDirPath, recursive);

        return new {
          success = true,
          operation = "delete-directory",
          directoryName = dirInfo.Name,
          path = relativePath,
          deletedAt = DateTime.Now,
          recursive = recursive,
          filesDeleted = fileCount,
          subdirectoriesDeleted = subdirCount,
          message = $"Directory deleted successfully: {path}"
        };
      } catch (Exception ex) when (!(ex is UnauthorizedAccessException || ex is ArgumentException || ex is DirectoryNotFoundException)) {
        throw new InvalidOperationException($"Failed to delete directory: {ex.Message}", ex);
      }
    }
  }
}
