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
  }
}
