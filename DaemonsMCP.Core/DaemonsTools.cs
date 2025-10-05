using DaemonsMCP.Core.Config;
using DaemonsMCP.Core.Extensions;
using DaemonsMCP.Core.Models;
using DaemonsMCP.Core.Repositories;
using DaemonsMCP.Core.Services;
using MCPSharp;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DaemonsMCP.Core {

  /// <summary>
  /// DaemonsTools is the DaemonsMCP primary interface. 
  /// </summary>
  /// <param name="config"></param>
  /// <param name="projectsService"></param>
  /// <param name="projectFolderService"></param>
  /// <param name="projectFileService"></param>
  public class DaemonsTools(
    IAppConfig config,
    IProjectsService projectsService,
    IProjectFolderService projectFolderService,
    IProjectFileService projectFileService,
    IIndexService IndexService,
    IClassService classService,
    ILoggerFactory loggerFactory
    ) {

    private readonly IAppConfig _config = config ?? throw new ArgumentNullException(Cx.ErrorNoConfig);
    private readonly IProjectsService _projectsService = projectsService;
    private readonly IProjectFolderService _projectFolderService = projectFolderService;    
    private readonly IProjectFileService _projectFileService = projectFileService;
    private readonly IIndexService _indexService = IndexService;
    private readonly IClassService _classService = classService;
    private readonly ILogger<DaemonsTools> _logger = loggerFactory.CreateLogger<DaemonsTools>();

    #region Projects 

    [McpTool(Cx.ListProjectsCmd, Cx.ListProjectsDesc)]
    public async Task<object> ListProjects() {
      try { 
        var projects = await _projectsService.GetProjectsAsync().ConfigureAwait(false);
        return JsonSerializer.Serialize(new { projects = projects.ToArray() });
      } catch (Exception ex) {
        _logger.LogError(ex, "Error listing projects");
        var opResult = OperationResult.CreateFailure(Cx.ListProjectsCmd,$"Failed: {ex.Message}",null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    #endregion

    #region Project Folders
    [McpTool(Cx.ListFoldersCmd, Cx.ListFoldersDesc)]
    public async Task<object> ListProjectDirectories(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.FolderPathParamDesc)] string? path = null,
        [Description(Cx.FolderFilterParamDesc)] string? filter = null) {
      try { 
        var directories = await _projectFolderService.GetFoldersAsync(projectName, path, filter).ConfigureAwait(false);
        var opResult = OperationResult.CreateSuccess(Cx.ListFoldersCmd, $"{Cx.ListFoldersCmd} Success.", directories.ToArray<string>());
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, path == null ? "Error listing project root directories" : $"Error listing directories in path: {path}");
        var opResult = OperationResult.CreateFailure(Cx.ListFoldersCmd, $"Failed: {ex.Message}", null);
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
        _logger.LogError(ex, $"Error creating directory: {path} in project: {projectName}");
        var opResult = OperationResult.CreateFailure(Cx.CreateFolderCmd, $"Failed: {ex.Message}", null);
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
        _logger.LogError(ex, $"Error deleting directory: {path} in project: {projectName}");
        var opResult = OperationResult.CreateFailure(Cx.DeleteFolderCmd, $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    #endregion

    #region Project Files
    [McpTool(Cx.ListFilesCmd, Cx.ListFilesDesc)]
    public async Task<object> ListProjectFiles(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.FilePathParamDesc)] string? path = null,
        [Description(Cx.FileFilterParamDesc)] string? filter = null) {
      try {      
        var files = await _projectFileService.GetFilesAsync(projectName, path, filter).ConfigureAwait(false);
        var opResult = OperationResult.CreateSuccess(Cx.ListFilesCmd, $"{Cx.ListFilesCmd} Success.", files.ToArray<string>());
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, path == null ? "Error listing project root files" : $"Error listing files in path: {path}");
        var opResult = OperationResult.CreateFailure(Cx.ListFilesCmd, $"Failed: {ex.Message}", null);
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
        _logger.LogError(ex, $"Error getting file: {path} in project: {projectName}");
        var opResult = OperationResult.CreateFailure(Cx.GetFileCmd, $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    [McpTool(Cx.InsertFileCmd, Cx.InsertFileDesc)]
    public async Task<object> CreateProjectFile(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.FilePathParamDesc)] string path,
        [Description(Cx.FileContentParamDesc)] string content) {
      try {

        var result = await _projectFileService.CreateFileAsync(projectName, path, content).ConfigureAwait(false);
        return JsonSerializer.Serialize(result);
      } catch (Exception ex) {
        _logger.LogError(ex, $"Error creating file: {path} in project: {projectName}");
        var opResult = OperationResult.CreateFailure(Cx.InsertFileCmd, $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }

        
    }

    [McpTool(Cx.UpdateFileCmd, Cx.UpdateFileDesc)]
    public async Task<object> UpdateProjectFile(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.FilePathParamDesc)] string path,
        [Description(Cx.FileContentParamDesc)] string content) {
      try { 
        var result = await _projectFileService.UpdateFileAsync(projectName, path, content).ConfigureAwait(false);
        return JsonSerializer.Serialize(result);
      } catch (Exception ex) {
        _logger.LogError(ex, $"Error updating file: {path} in project: {projectName}");
        var opResult = OperationResult.CreateFailure(Cx.UpdateFileCmd, $"Failed: {ex.Message}", null);
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
        _logger.LogError(ex, $"Error deleting file: {path} in project: {projectName}");
        var opResult = OperationResult.CreateFailure(Cx.DeleteFileCmd, $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }
    #endregion

    #region Index Operations 
        
    public async Task<object> RebuildIndex( bool forceRebuild = false) {
      try { 
        var result = await _indexService.RebuildIndexAsync(forceRebuild).ConfigureAwait(false);
        return JsonSerializer.Serialize(result);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error rebuilding index");
        var opResult = OperationResult.CreateFailure(Cx.ResyncIndexCmd, $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<object> GetIndexStatus() {
      try { 
        var status = await _indexService.GetIndexStatus().ConfigureAwait(false);
        return JsonSerializer.Serialize(status);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error getting index status");
        var opResult = OperationResult.CreateFailure(Cx.StatusIndexCmd, $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<object> ChangeIndexStatus( bool enabled) {
      try { 
        _indexService.Enabled = enabled;
        var status = await _indexService.GetIndexStatus().ConfigureAwait(false);
        return JsonSerializer.Serialize(status);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error changing index status");
        var opResult = OperationResult.CreateFailure(Cx.ChangeStatusIndexCmd, $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    #endregion

    #region Class Operations

    public async Task<object> GetClassesAsync(string projectName, int pageNo =1, int itemsPerPage=20, string? namespaceFilter = null, string? classNameFilter = null) {
      try {
        var result = await _classService.GetClassesAsync(projectName, pageNo, itemsPerPage, namespaceFilter, classNameFilter).ConfigureAwait(false);
        return JsonSerializer.Serialize(result);
      } catch (Exception ex) {
        _logger.LogError(ex,"Error getting classes in project: {ProjectName}", projectName);
        var opResult = OperationResult.CreateFailure(Cx.ListClassesCmd, $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<object> GetClassContentAsync(string projectName, int classId) {
      try {
        var result = await _classService.GetClassContentAsync(projectName, classId).ConfigureAwait(false);
        return JsonSerializer.Serialize(result);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error getting class content in project: {ProjectName}, classId: {ClassId}", projectName, classId);
        var opResult = OperationResult.CreateFailure(Cx.GetClassCmd, $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<object> AddUpdateClassAsync(string projectName, ClassContent content) {
      try { 
        var result = await _classService.AddUpdateClassContentAsync(projectName, content).ConfigureAwait(false);
        return JsonSerializer.Serialize(result);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error adding/updating class content in project: {ProjectName}, classId: {ClassId}", projectName, content.ClassId);
        var opResult = OperationResult.CreateFailure(Cx.AddUpdateClassCmd, $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    #endregion

    #region Method Operations

    public async Task<object> GetMethodsAsync(string projectName, int pageNo = 1, int itemsPerPage = 20, string? namespaceFilter = null, string? classNameFilter = null, string? methodNameFilter = null) {
      try { 
        var result = await _classService.GetMethodsAsync(projectName, pageNo, itemsPerPage, namespaceFilter, classNameFilter, methodNameFilter).ConfigureAwait(false);
        return JsonSerializer.Serialize(result);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error getting methods in project: {ProjectName}", projectName);
        var opResult = OperationResult.CreateFailure(Cx.ListMethodsCmd, $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<object> GetMethodContentAsync(string projectName, int methodId) {
      try { 
        var result = await _classService.GetMethodContentAsync(projectName, methodId).ConfigureAwait(false);
        return JsonSerializer.Serialize(result);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error getting method content in project: {ProjectName}, methodId: {MethodId}", projectName, methodId);
        var opResult = OperationResult.CreateFailure(Cx.GetMethodCmd, $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<object> AddUpdateMethodAsync(string projectName, MethodContent content) {
      try { 
        var result = await _classService.AddUpdateMethodAsync(projectName, content).ConfigureAwait(false);
        return JsonSerializer.Serialize(result);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error adding/updating method content in project: {ProjectName}, methodId: {MethodId}", projectName, content.MethodId);
        var opResult = OperationResult.CreateFailure(Cx.AddUpdateMethodCmd, $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    #endregion

    #region Nodes Operations 

    public async Task<object> GetItemTypes() {
      try { 
        var itemTypes = await _projectsService.GetItemTypes().ConfigureAwait(false);
        return JsonSerializer.Serialize(itemTypes);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error Getting Types");
        var opResult = OperationResult.CreateFailure("GetItemTypes", $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<object> AddUpdateItemType(ItemType itemType) {
      try { 
        var result = await _projectsService.AddUpdateItemType(itemType);
        return JsonSerializer.Serialize(result);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error Adding/Updating Item Type");
        var opResult = OperationResult.CreateFailure("AddUpdateItemType", $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<object> GetStatusTypes() {
      try { 
        var statusTypes = await _projectsService.GetStatusTypes().ConfigureAwait(false);
        return JsonSerializer.Serialize(statusTypes);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error Getting Status Types");
        var opResult = OperationResult.CreateFailure("GetStatusTypes", $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<object> AddUpdateStatusType(StatusType statusType) {
      try { 
        var result = await _projectsService.AddUpdateStatusType(statusType).ConfigureAwait(false);
        return JsonSerializer.Serialize(result);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error Adding/Updating Status Type");
        var opResult = OperationResult.CreateFailure("AddUpdateStatusType", $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<object> GetNodes(int? nodeId = null, int maxDepth = 1, string? statusFilter = null, string? typeFilter = null, string? nameContains = null, string? detailsContains = null) {
      try { 
        var nodes = await _projectsService.GetNodes( nodeId, maxDepth, statusFilter, typeFilter, nameContains, detailsContains).ConfigureAwait(false);
        return JsonSerializer.Serialize(nodes);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error Getting Nodes");
        var opResult = OperationResult.CreateFailure("GetNodes", $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<object> GetNodeById( int nodeId) {
      try { 
        var node = await _projectsService.GetNodeById(nodeId).ConfigureAwait(false);
        return JsonSerializer.Serialize(node);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error Getting Node By Id: {NodeId}", nodeId);
        var opResult = OperationResult.CreateFailure("GetNodeById", $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<object> AddUpdateNode( Nodes node) {
      try { 
        var result = await _projectsService.AddUpdateNode( node).ConfigureAwait(false);
        return JsonSerializer.Serialize(result);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error Adding/Updating Node Id: {NodeId}", node.Id);
        var opResult = OperationResult.CreateFailure("AddUpdateNode", $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<object> AddUpdateNodeList(List<Nodes> nodes) {
      try { 
        var result = await _projectsService.AddUpdateNodeList( nodes).ConfigureAwait(false);
        return JsonSerializer.Serialize(result);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error Adding/Updating Node List, count: {NodeCount}", nodes.Count);
        var opResult = OperationResult.CreateFailure("AddUpdateNodeList", $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<object> RemoveNode( int nodeId, RemoveStrategy removeStrategy = RemoveStrategy.PreventIfHasChildren) {
      try { 
        var result = await _projectsService.RemoveNode( nodeId, removeStrategy).ConfigureAwait(false);
        return JsonSerializer.Serialize(result);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error Removing Node Id: {NodeId}", nodeId);
        var opResult = OperationResult.CreateFailure("RemoveNode", $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<object> SaveProjectRepo() {
      try {
        var opResult = await _projectsService.SaveProjectRepo().ConfigureAwait(false);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) { 
        _logger.LogError(ex, "Error Saving Project Repo");
        var opResult = OperationResult.CreateFailure(Cx.SaveProjectRepoCmd, "failed", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    #endregion
    #region Todo Operations

    public async Task<object> MakeTodoList(string listName, string[] items) {
      try { 
        var result = await _projectsService.MakeTodoList(listName, items).ConfigureAwait(false);
        return JsonSerializer.Serialize(result);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error Making Todo List: {ListName}", listName);
        var opResult = OperationResult.CreateFailure("MakeTodoList", $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<object> GetNextTodoItem(string listName) {
      try { 
        var result = await _projectsService.GetNextTodoItem(listName).ConfigureAwait(false);
        return JsonSerializer.Serialize(result);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error Getting Next Todo Item from List: {ListName}", listName);
        var opResult = OperationResult.CreateFailure("GetNextTodoItem", $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<object> MarkTodoDone(int itemId) {
      try { 
        var result = await _projectsService.MarkTodoDone(itemId).ConfigureAwait(false);
        return JsonSerializer.Serialize(result);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error Marking Todo Item Done, ItemId: {ItemId}", itemId);
        var opResult = OperationResult.CreateFailure("MarkTodoDone", $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<object> RestoreAsTodo(int itemId) {
      try { 
        var result = await _projectsService.RestoreAsTodo(itemId).ConfigureAwait(false);
        return JsonSerializer.Serialize(result);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error Restoring Todo Item, ItemId: {ItemId}", itemId);
        var opResult = OperationResult.CreateFailure("RestoreAsTodo", $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<object> MarkTodoCancel(int itemId) {
      try { 
        var result = await _projectsService.MarkTodoCancel(itemId).ConfigureAwait(false);
        return JsonSerializer.Serialize(result);
      } catch (Exception ex) {
        _logger.LogError(ex,"Error Cancelling Todo Item, ItemId: {ItemId}", itemId);
        var opResult = OperationResult.CreateFailure("MarkTodoCancel", $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }


    #endregion

  }
}
