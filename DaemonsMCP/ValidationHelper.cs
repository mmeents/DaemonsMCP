using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP {
  public static class ValidationHelper {

    public static Project GetValidatedProject(string projectName) {
      if (string.IsNullOrWhiteSpace(projectName)) {
        throw new ArgumentException("Project name is required", nameof(projectName));
      }
      if (!GlobalConfig.Projects.TryGetValue(projectName, out Project? project))
        throw new ArgumentException($"Invalid project name: {projectName}");
      return project;
    }

    public static string BuildAndValidatePath(Project project, string relativePath, bool IsDirectory) {

      var fullPath = Path.GetFullPath(Path.Combine(project.Path, relativePath));

      // SAFETY CHECK: Ensure path is within project boundaries
      var normalizedProjectPath = Path.GetFullPath(project.Path);
      if (!fullPath.StartsWith(normalizedProjectPath, StringComparison.OrdinalIgnoreCase))
        throw new UnauthorizedAccessException((IsDirectory? "Directory" : "File")+" path must be within the project directory");

      return fullPath;
    }

    public static void ValidateProjectContentInput(string projectName, string path, string? content = null) {

      if (string.IsNullOrWhiteSpace(path))
        throw new ArgumentException("File path is required", nameof(path));

      if (content == null) 
        throw new ArgumentException("Content cannot be null", nameof(content));
    }
  }
}
