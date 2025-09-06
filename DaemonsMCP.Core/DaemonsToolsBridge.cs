using DaemonsMCP.Core.Extensions;
using DaemonsMCP.Core.Models;
using MCPSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core {
  /// <summary>
  /// MCPSharp-compatible wrapper that delegates to DI-resolved DaemonsTools
  /// </summary>
  public class DaemonsToolsBridge {
    private static DaemonsTools GetTools() => DIServiceBridge.GetService<DaemonsTools>();

    [McpTool(Cx.ListProjectsCmd, Cx.ListProjectsDesc)]
    public static async Task<object> ListProjects() => await GetTools().ListProjects();


    #region Project Folders
    [McpTool(Cx.ListFoldersCmd, Cx.ListFoldersDesc)]
    public static async Task<object> ListProjectDirectories(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.FolderPathParamDesc)] string? path = null,
        [Description(Cx.FolderFilterParamDesc)] string? filter = null)
        => await GetTools().ListProjectDirectories(projectName, path, filter).ConfigureAwait(false);

    [McpTool(Cx.CreateFolderCmd, Cx.CreateFolderDesc)]
    public static async Task<object> CreateProjectDirectory(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.FolderPathParamDesc)] string path,
        [Description(Cx.CreateFolderOptionDesc)] bool createDirectories = true)
        => await GetTools().CreateProjectDirectory(projectName, path, createDirectories).ConfigureAwait(false);

    [McpTool(Cx.DeleteFolderCmd, Cx.DeleteFolderDesc)]
    public static async Task<object> DeleteProjectDirectory(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.FolderPathParamDesc)] string path,
        [Description(Cx.DeleteFolderOptionDesc)] bool deleteDirectories = true)
        => await GetTools().DeleteProjectDirectory(projectName, path, deleteDirectories).ConfigureAwait(false);
    #endregion

    #region Project Files
    [McpTool(Cx.ListFilesCmd, Cx.ListFilesDesc)]
    public static async Task<object> ListProjectFiles(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.FilePathParamDesc)] string? path = null,
        [Description(Cx.FileFilterParamDesc)] string? filter = null)
        => await GetTools().ListProjectFiles(projectName, path, filter).ConfigureAwait(false);

    [McpTool(Cx.GetFileCmd, Cx.GetFileDesc)]
    public static async Task<object> GetProjectFile(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.FilePathParamDesc)] string path)
        => await GetTools().GetProjectFile(projectName, path).ConfigureAwait(false);

    [McpTool(Cx.InsertFileCmd, Cx.InsertFileDesc)]
    public static async Task<object> CreateProjectFile(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.FilePathParamDesc)] string path,
        [Description(Cx.FileContentParamDesc)] string content)
        => await GetTools().CreateProjectFile(projectName, path, content).ConfigureAwait(false);

    [McpTool(Cx.UpdateFileCmd, Cx.UpdateFileDesc)]
    public static async Task<object> UpdateProjectFile(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.FilePathParamDesc)] string path,
        [Description(Cx.FileContentParamDesc)] string content)
        => await GetTools().UpdateProjectFile(projectName, path, content).ConfigureAwait(false);

    [McpTool(Cx.DeleteFileCmd, Cx.DeleteFileDesc)]
    public static async Task<object> DeleteProjectFile(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.FilePathParamDesc)] string path)
        => await GetTools().DeleteProjectFile(projectName, path).ConfigureAwait(false);
    #endregion

    #region Index

    [McpTool(Cx.ResyncIndexCmd, Cx.ResyncIndexCmdDesc)]
    public static async Task<object> RebuildIndex(
         [Description(Cx.ForceRebuildOptionDesc)] bool forceRebuild = false)
        => await GetTools().RebuildIndex(forceRebuild);

    [McpTool(Cx.StatusIndexCmd, Cx.StatusIndexCmdDesc)]
    public static async Task<object> GetIndexStatus()
        => await GetTools().GetIndexStatus();

    [McpTool(Cx.ChangeStatusIndexCmd, Cx.ChangeStatusIndexCmdDesc)]
    public static async Task<object> ChangeIndexStatus(
        [Description(Cx.ChangeStatusIndexEnableDesc)] bool enableIndex)  
      => await GetTools().ChangeIndexStatus(enableIndex);

    #endregion

    #region Class Operations

    [McpTool(Cx.ListClassesCmd, Cx.ListClassesCmdDesc)]
    public static async Task<object> GetClassesAsync(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.PageNoParamDesc)] int pageNo = 1,
        [Description(Cx.ItemsPerPageParamDesc)] int itemsPerPage = 100,
        [Description(Cx.NamespaceFilterParamDesc)] string? namespaceFilter = null,
        [Description(Cx.ClassFilterParamDesc)] string? classFilter = null) 
      => await GetTools().GetClassesAsync(projectName, pageNo, itemsPerPage, namespaceFilter, classFilter).ConfigureAwait(false);

    [McpTool(Cx.GetClassCmd, Cx.GetClassCmdDesc)]
    public static async Task<object> GetClassContentAsync(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.ClassIdParamDesc)] int classId) 
      => await GetTools().GetClassContentAsync(projectName, classId).ConfigureAwait(false);

    [McpTool(Cx.AddUpdateClassCmd, Cx.AddUpdateClassCmdDesc)]
    public static async Task<object> AddUpdateClassAsync(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.ClassContentParamDesc)] ClassContent classContent
        ) 
      => await GetTools().AddUpdateClassAsync(projectName, classContent).ConfigureAwait(false);


    #endregion

    #region Method Operations

    [McpTool(Cx.ListMethodsCmd, Cx.ListClassMethodsCmdDesc)]
    public static async Task<object> GetMethodsAsync(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.PageNoParamDesc)] int pageNo = 1,
        [Description(Cx.ItemsPerPageParamDesc)] int itemsPerPage = 100,
        [Description(Cx.NamespaceFilterParamDesc)] string? namespaceFilter = null,
        [Description(Cx.ClassFilterParamDesc)] string? classFilter = null,
        [Description(Cx.MethodFilterParamDesc)] string? methodFilter = null)
      => await GetTools().GetMethodsAsync(projectName, pageNo, itemsPerPage, namespaceFilter, classFilter, methodFilter).ConfigureAwait(false);

    [McpTool(Cx.GetClassMethodCmd, Cx.GetClassMethodCmdDesc)]
    public static async Task<object> GetMethodContentAsync(
      [Description(Cx.ProjectNameParamDesc)] string projectName,
      [Description(Cx.MethodIdParamDesc)] int methodId
    ) => await GetTools().GetMethodContentAsync(projectName, methodId).ConfigureAwait(false);

    [McpTool(Cx.AddUpdateMethodCmd, Cx.AddUpdateMethodCmdDesc)]
    public static async Task<object> AddUpdateMethodAsync(
      [Description(Cx.ProjectNameParamDesc)] string projectName,
      [Description(Cx.MethodContentParamDesc)] MethodContent methodContent
    ) => await GetTools().AddUpdateMethodAsync(projectName, methodContent).ConfigureAwait(false);


    #endregion

    #region Nodes Operations

    [McpTool(Cx.GetItemTypesCmd, Cx.GetItemTypesCmdDesc)]
    public static async Task<object> GetItemTypes([Description(Cx.ProjectNameParamDesc)] string projectName)
      => await GetTools().GetItemTypes(projectName).ConfigureAwait(false);

    [McpTool(Cx.AddUpdateItemTypeCmd, Cx.AddUpdateItemTypeCmdDesc)]
    public static async Task<object> AddUpdateItemType(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.ItemTypeParamDesc)] ItemType type )
      => await GetTools().AddUpdateItemType(projectName, type).ConfigureAwait(false);

    [McpTool(Cx.GetStatusTypesCmd, Cx.GetStatusTypesCmdDesc)]
    public static async Task<object> GetStatusTypes([Description(Cx.ProjectNameParamDesc)] string projectName)
      => await GetTools().GetStatusTypes(projectName).ConfigureAwait(false);

    [McpTool(Cx.AddUpdateStatusTypeCmd, Cx.AddUpdateStatusTypeCmdDesc)]
    public static async Task<object> AddUpdateStatusType(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.StatusTypeParamDesc)] StatusType status)
      => await GetTools().AddUpdateStatusType(projectName, status).ConfigureAwait(false);

    [McpTool(Cx.GetNodesCmd, Cx.GetNodesCmdDesc)]
    public static async Task<object> GetNodes(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.NodeIdParamDesc)] int? nodeId = null,
        [Description(Cx.MaxDepthParamDesc)] int maxDepth = 1,
        [Description(Cx.StatusFilterParamDesc)] string? statusFilter = null,
        [Description(Cx.TypeFilterParamDesc)] string? typeFilter = null,
        [Description(Cx.NameContainsParamDesc)] string? nameContains = null,
        [Description(Cx.DetailsContainsParamDesc)] string? detailsContains = null)
      => await GetTools().GetNodes(projectName, nodeId, maxDepth, statusFilter, typeFilter, nameContains, detailsContains).ConfigureAwait(false);

    [McpTool(Cx.GetNodesByIdCmd, Cx.GetNodesByIdCmdDesc)]
    public static async Task<object> GetNodesById(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.NodeIdParamDesc)] int nodeIds)
      => await GetTools().GetNodeById(projectName, nodeIds).ConfigureAwait(false);

    [McpTool(Cx.AddUpdateNodesCmd, Cx.AddUpdateNodesCmdDesc)]
    public static async Task<object> AddUpdateNodes(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.NodesTreeParamDesc)] Nodes node)
      => await GetTools().AddUpdateNode(projectName, node).ConfigureAwait(false);

    [McpTool(Cx.AddUpdateNodesListCmd, Cx.AddUpdateNodesListCmdDesc)]
    public static async Task<object> AddUpdateNodesList(
      [Description(Cx.ProjectNameParamDesc)] string projectName,
      [Description(Cx.NodesListTreeParamDesc)] List<Nodes> nodes
      ) => await GetTools().AddUpdateNodeList(projectName, nodes).ConfigureAwait(false);

    [McpTool(Cx.RemoveNodeCmd, Cx.RemoveNodeCmdDesc)]
    public static async Task<object> RemoveNode(
      [Description(Cx.ProjectNameParamDesc)] string projectName,
      [Description(Cx.NodeIdParamDesc)] int nodeId,
      [Description(Cx.RemoveStrategyParamDesc)] RemoveStrategy removeStrategy
    ) => await GetTools().RemoveNode(projectName, nodeId, removeStrategy).ConfigureAwait(false);

    [McpTool(Cx.SaveProjectRepoCmd, Cx.SaveProjectRepoCmdDesc)]
    public static async Task<object> SaveProjectRepo([Description(Cx.ProjectNameParamDesc)] string projectName)
      => await GetTools().SaveProjectRepo(projectName).ConfigureAwait(false);

    

    #endregion
  }
}
