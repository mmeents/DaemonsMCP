using DaemonsMCP.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaemonsMCP.Core.Config;

namespace DaemonsMCP.Core.Services {
  public class ValidationService : IValidationService {
    private readonly IAppConfig _config;
    private readonly ISecurityService _securityService;
    public ValidationService(IAppConfig config, ISecurityService securityService) {
      _config = config ?? throw new ArgumentNullException(nameof(config), "Configuration cannot be null");
      _securityService = securityService;
    }

    public ValidationContext ValidateAndPrepare(string projectName, string path, bool ItemIsDir) { 
      if (string.IsNullOrWhiteSpace(projectName)) {
        throw new ArgumentException("Project name cannot be null or empty", nameof(projectName));
      }
      ProjectModel? project;
      if (!_config.Projects.TryGetValue(projectName, out project) ){ 
        throw new ArgumentException($"Project '{projectName}' not found", nameof(projectName));
      }
      if (string.IsNullOrWhiteSpace(path)) {
        path = string.Empty; // Default to project root if no path is provided
      }
      var fullPath = BuildAndValidatePath(project, path, ItemIsDir);

      ValidationContext validationContext = new ValidationContext() { 
        Project = project,
        RelativePath = path ?? string.Empty,
        FullPath = fullPath
      };

      return validationContext;
    }

    public string BuildAndValidatePath(ProjectModel project, string relativePath, bool isDirectory = false) {
      try {
        var fullPath = string.IsNullOrEmpty(relativePath) ? project.Path : Path.Combine(project.Path, relativePath);
        fullPath = Path.GetFullPath(fullPath);
        var normalizedProjectPath = Path.GetFullPath(project.Path);

        if (!fullPath.StartsWith(normalizedProjectPath, StringComparison.OrdinalIgnoreCase))
          throw new UnauthorizedAccessException("Path must be within project boundaries");

        if (!isDirectory && !File.Exists(fullPath)) { 
          throw new FileNotFoundException($"File not found: {fullPath}");
        }
        if (!isDirectory && !_securityService.IsFileAllowed(fullPath)) {
          throw new UnauthorizedAccessException("GetFileItemById Access to this file type is not allowed for security reasons.");
        }

        if (isDirectory && !Directory.Exists(fullPath)) { 
          Directory.CreateDirectory(fullPath);
          if (!Directory.Exists(fullPath)) {
            throw new DirectoryNotFoundException($"Directory not found or could not be created: {fullPath}");
          }
        }

        return fullPath;

      } catch (Exception ex) when (!(ex is UnauthorizedAccessException)) {
        throw new ArgumentException($"Invalid path: {ex.Message}", nameof(relativePath));
      }
    }

    public void ValidateProjectName(string projectName) {
      if (string.IsNullOrWhiteSpace(projectName))
        throw new ArgumentException("Project name is required", nameof(projectName));
    }

    public void ValidatePath(string path) {
      if (string.IsNullOrWhiteSpace(path))
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

    public void ValidatePrepToSave(string path, string fullPath, string content, bool overwrite) {

      // Check if file already exists
      if (File.Exists(fullPath) && !overwrite) {
        throw new InvalidOperationException($"File already exists: {path}. Use overwrite=true to replace it.");
      }

      // Security validations
      if (!_securityService.IsWriteAllowed(fullPath)) {
        throw new UnauthorizedAccessException("Write operation not allowed for security reasons");
      }

      if (!_securityService.IsWriteContentSizeAllowed(Encoding.UTF8.GetByteCount(content))) {
        throw new ArgumentException("Content size exceeds maximum allowed for write operations");
      }
    }

    public void ValidateClassContent(ClassContent content) { 
      if (content == null)
          throw new ArgumentException("Class content cannot be null", nameof(content));
      if (string.IsNullOrWhiteSpace(content.Namespace))
          throw new ArgumentException("Namespace is required", nameof(content.Namespace));
      if (string.IsNullOrWhiteSpace(content.ClassName))
          throw new ArgumentException("Class name is required", nameof(content.ClassName));
      if (string.IsNullOrWhiteSpace(content.Content))
          throw new ArgumentException("Class content cannot be empty", nameof(content.Content));
    }
  }
}
