using DaemonsMCP.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaemonsMCP.Core.Config;
using DaemonsMCP.Core.Extensions;
using OperationResult = DaemonsMCP.Core.Models.OperationResult;


namespace DaemonsMCP.Core.Services {
  public class ProjectFolderService(
      IAppConfig config,
      IValidationService validationService,
      ISecurityService securityService) : IProjectFolderService {
    private readonly IAppConfig _config = config;
    private readonly IValidationService _validationService = validationService;
    private readonly ISecurityService _securityService = securityService;

    public Task<IEnumerable<string>> GetFoldersAsync(string projectName, string? path = null, string? filter = null) {
      var context = _validationService.ValidateAndPrepare(projectName, path ?? "", true, true);
      var fullPath = context.FullPath;

      var searchPattern = string.IsNullOrEmpty(filter) ? "*" : filter;
      var directories = Directory.GetDirectories(fullPath, searchPattern, SearchOption.TopDirectoryOnly);

      var relativeDirs = directories.Select(dir =>
          dir.Substring(context.Project.Path.Length).TrimStart(Path.DirectorySeparatorChar)
      );

      return Task.FromResult(relativeDirs);
    }

    public Task<OperationResult> CreateFolderAsync(string projectName, string path, bool createParents = true) {
      try {
        var context = _validationService.ValidateAndPrepare(projectName, path, true, true);                       
        var fullDirPath = context.FullPath;
      
        // Check if path is write-protected
        if (_securityService.IsPathWriteProtected(fullDirPath)) {
          throw new UnauthorizedAccessException("Cannot create directory in write-protected location");
        }

        // Check if directory already exists
        if (Directory.Exists(fullDirPath)) {
          throw new InvalidOperationException($"Directory already exists: {path}");
        }

      
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
        var relativePath = dirInfo.FullName.Substring(context.Project.Path.Length).TrimStart(Path.DirectorySeparatorChar);

        var OpResult = OperationResult.CreateSuccess(
          Cx.CreateFolderCmd, 
          $"Directory created successfully: {path}",
          new {
            directoryName = dirInfo.Name,
            path = relativePath,
            created = dirInfo.CreationTime,
          }
        );

        return Task.FromResult(OpResult);
      } catch (Exception ex) when (!(ex is UnauthorizedAccessException || ex is ArgumentException || ex is InvalidOperationException)) {
        var opResult = OperationResult.CreateFailure(
          Cx.CreateFolderCmd,
          $"Failed to create directory: {path}",
          ex
        );
        return Task.FromResult(opResult);
      }
    }

    public Task<OperationResult> DeleteFolderAsync(string projectName, string path, bool recursive = false, bool confirmDeletion = false) {
      try {
        // SAFETY: Require explicit confirmation
        if (!confirmDeletion) {
          throw new ArgumentException("Directory deletion requires explicit confirmation. Set confirmDeletion=true to proceed.");
        }

        var context = _validationService.ValidateAndPrepare(projectName, path, true, true);
        var fullDirPath = context.FullPath;

        // Check if directory exists
        if (!Directory.Exists(fullDirPath)) {
          throw new DirectoryNotFoundException($"Directory not found: {fullDirPath}");
        }

        // SAFETY: Extra protection for critical directories
        var protectedDirs = new[] { ".git", ".vs", "bin", "obj", "Properties" };
        if (protectedDirs.Any(dir => path.Equals(dir, StringComparison.OrdinalIgnoreCase) ||
                                     path.StartsWith(dir + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))) {
          throw new UnauthorizedAccessException($"Cannot delete protected directory: {path}");
        }
      
        var dirInfo = new DirectoryInfo(fullDirPath);
        var dirName = dirInfo.Name;
        var relativePath = dirInfo.FullName.Substring(context.Project.Path.Length).TrimStart(Path.DirectorySeparatorChar);
        var fileCount = recursive ? Directory.GetFiles(fullDirPath, "*", SearchOption.AllDirectories).Length : 0;
        var subdirCount = recursive ? Directory.GetDirectories(fullDirPath, "*", SearchOption.AllDirectories).Length : 0;

        // DeleteClassItem directory
        Directory.Delete(fullDirPath, recursive);

        var opResult = OperationResult.CreateSuccess(
          Cx.DeleteFolderCmd,
          $"Directory deleted successfully: {path}",
          new {
            directoryName = dirName,
            path = relativePath,
            deletedAt = DateTime.Now,
            recursive = recursive,
            filesDeleted = fileCount,
            subdirectoriesDeleted = subdirCount
          }
        );

        return Task.FromResult(opResult);
      } catch (Exception ex) when (!(ex is UnauthorizedAccessException || ex is ArgumentException || ex is DirectoryNotFoundException)) {
        var opResult = OperationResult.CreateFailure(
          Cx.DeleteFolderCmd,
          $"Failed to delete directory: {path} {ex.Message}",
          ex
        );
        return Task.FromResult(opResult);
      }
    }
  }
     
}
