using DaemonsMCP.Core.Config;
using DaemonsMCP.Core.Extensions;
using DaemonsMCP.Core.Models;
using DaemonsMCP.Core.Services;
using MCPSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DaemonsMCP.Core {
  public class DaemonsTools(
    IAppConfig config,
    IProjectsService projectsService,
    IProjectFolderService projectFolderService,
    IProjectFileService projectFileService
    ) {

    private readonly IAppConfig _config = config ?? throw new ArgumentNullException(Cx.ErrorNoConfig);
    private readonly IProjectsService _projectsService = projectsService;
    private readonly IProjectFolderService _projectFolderService = projectFolderService;    
    private readonly IProjectFileService _projectFileService = projectFileService;

    // Projects 

    [McpTool(Cx.ListProjectsCmd, Cx.ListProjectsDesc)]
    public async Task<object> ListProjects() {
      try { 
        var projects = await _projectsService.GetProjectsAsync().ConfigureAwait(false);
        return JsonSerializer.Serialize(new { projects = projects.ToArray() });
      } catch (Exception ex) {
        var opResult = OperationResult.CreateFailure(Cx.ListProjectsCmd,$"Failed: {ex.Message}",ex);
        return JsonSerializer.Serialize(opResult);
      }
    }

    // Project Folders
    [McpTool(Cx.ListFoldersCmd, Cx.ListFoldersDesc)]
    public async Task<object> ListProjectDirectories(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.FolderPathParamDesc)] string? path = null,
        [Description(Cx.FolderFilterParamDesc)] string? filter = null) {
      try { 
        var directories = await _projectFolderService.GetFoldersAsync(projectName, path, filter).ConfigureAwait(false);
        return JsonSerializer.Serialize(new { directories = directories.ToArray() });
      } catch (Exception ex) {
        var opResult = OperationResult.CreateFailure(Cx.ListFoldersCmd, $"Failed: {ex.Message}", ex);
        return JsonSerializer.Serialize(opResult);
      }
    }

    [McpTool(Cx.CreateFolderCmd, Cx.CreateFolderDesc)]
    public async Task<object> CreateProjectDirectory(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.FolderPathParamDesc)] string path,
        [Description(Cx.CreateFolderOptionDesc)] bool createDirectories = true) {
      try { 
        var result = await _projectFolderService.CreateFolderAsync(projectName, path, createDirectories).ConfigureAwait(false);

        return JsonSerializer.Serialize(result);
      } catch (Exception ex) {
        var opResult = OperationResult.CreateFailure(Cx.CreateFolderCmd, $"Failed: {ex.Message}", ex);
        return JsonSerializer.Serialize(opResult);
      }
    }

    [McpTool(Cx.DeleteFolderCmd, Cx.DeleteFolderDesc)]
    public async Task<object> DeleteProjectDirectory(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.FolderPathParamDesc)] string path,
        [Description(Cx.CreateFolderOptionDesc)] bool deleteDirectories = true) {
      try { 
        var result = await _projectFolderService.DeleteFolderAsync(projectName, path, deleteDirectories).ConfigureAwait(false);
        return JsonSerializer.Serialize(result);

      } catch (Exception ex) {
        var opResult = OperationResult.CreateFailure(Cx.DeleteFolderCmd, $"Failed: {ex.Message}", ex);
        return JsonSerializer.Serialize(opResult);
      }
    }


    // Project Files
    [McpTool(Cx.ListFilesCmd, Cx.ListFilesDesc)]
    public async Task<object> ListProjectFiles(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.FilePathParamDesc)] string? path = null,
        [Description(Cx.FileFilterParamDesc)] string? filter = null) {
      try {      
        var files = await _projectFileService.GetFilesAsync(projectName, path, filter).ConfigureAwait(false);
        return JsonSerializer.Serialize(new { files = files.ToArray() });
      } catch (Exception ex) {
        var opResult = OperationResult.CreateFailure(Cx.ListFilesCmd, $"Failed: {ex.Message}", ex);
        return JsonSerializer.Serialize(opResult);
      }
    }

    [McpTool(Cx.GetFileCmd, Cx.GetFileDesc)]
    public async Task<object> GetProjectFile(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.FilePathParamDesc)] string path) {
      try { 
        var fileContent = await _projectFileService.GetFileAsync(projectName, path).ConfigureAwait(false);
        return JsonSerializer.Serialize(fileContent); 
      } catch (Exception ex) {
        var opResult = OperationResult.CreateFailure(Cx.GetFileCmd, $"Failed: {ex.Message}", ex);
        return JsonSerializer.Serialize(opResult);
      }
    }

    [McpTool(Cx.InsertFileCmd, Cx.InsertFileDesc)]
    public async Task<object> CreateProjectFile(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.FilePathParamDesc)] string path,
        [Description(Cx.FileContentParamDesc)] string content,
        [Description(Cx.CreateFolderOptionDesc)] bool createDirectories = true,
        [Description(Cx.OverwriteFileOptionDesc)] bool overwrite = false) {
      var result = await _projectFileService.CreateFileAsync(projectName, path, content, createDirectories, overwrite).ConfigureAwait(false);

      if (!result.Success)
        throw new InvalidOperationException(result.ErrorMessage, result.Exception);

      return JsonSerializer.Serialize(result);
    }

    [McpTool(Cx.UpdateFileCmd, Cx.UpdateFileDesc)]
    public async Task<object> UpdateProjectFile(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.FilePathParamDesc)] string path,
        [Description(Cx.FileContentParamDesc)] string content,
        [Description(Cx.CreateFolderOptionDesc)] bool createBackup = true) {
      try { 
        var result = await _projectFileService.UpdateFileAsync(projectName, path, content, createBackup).ConfigureAwait(false);
        return JsonSerializer.Serialize(result);
      } catch (Exception ex) {
        var opResult = OperationResult.CreateFailure(Cx.UpdateFileCmd, $"Failed: {ex.Message}", ex);
        return JsonSerializer.Serialize(opResult);
      }
    }

    [McpTool(Cx.DeleteFileCmd, Cx.DeleteFileDesc)]
    public async Task<object> DeleteProjectFile(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.FilePathParamDesc)] string path) {
      try { 
        var result = await _projectFileService.DeleteFileAsync(projectName, path).ConfigureAwait(false);
        return JsonSerializer.Serialize(result);
      } catch (Exception ex) {
        var opResult = OperationResult.CreateFailure(Cx.DeleteFileCmd, $"Failed: {ex.Message}", ex);
        return JsonSerializer.Serialize(opResult);
      }
    }


  }
}
