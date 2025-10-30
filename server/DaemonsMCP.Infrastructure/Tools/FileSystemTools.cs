using DaemonsMCP.Domain.Constants;
using DaemonsMCP.Domain.Extensions;
using MCPSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DaemonsMCP.Infrastructure.Tools {
  public class FileSystemTools {
    private static IFileSystemToolsHandler GetTools() => DIServiceBridge.GetService<IFileSystemToolsHandler>();

    [McpTool(Cx.ListFileSystemCmd, Cx.ListFileSystemDesc)]
    public static async Task<string> SearchFileSystem(
        [Description(Cx.ProjectParamDesc)] int projectId,
        [Description(Cx.SearchFilterParamDesc)] string? filter,
        [Description(Cx.SearchIncludeDirectoriesDesc)] bool includeDirectories = true,
        [Description(Cx.SearchIncludeFiles)] bool includeFiles = true,
        [Description(Cx.PageNoParamDesc)] int pageNo = 1,
        [Description(Cx.ItemsPerPageParamDesc)] int pageSize = 20 
    ){
      return await GetTools().SearchFileSystem(
        projectId,
        filter,
        includeDirectories,
        includeFiles,
        pageNo,
        pageSize );

    }

    [McpTool(Cx.GetFileCmd, Cx.GetFileDesc)]
    public static async Task<string> GetFile(
      [Description(Cx.ProjectParamDesc)] int projectId,
      [Description(Cx.FileSystemNodeIdParamDesc)] int fileSystemNodeId
    ) {
      return await GetTools().GetFile(projectId, fileSystemNodeId);
    }

    [McpTool(Cx.InsertFileCmd, Cx.InsertFileDesc)]
    public static async Task<string> CreateProjectFile(
    [Description(Cx.ProjectParamDesc)] int projectId,
    [Description(Cx.FilePathParamDesc)] string relativePath,
    [Description(Cx.FileContentParamDesc)] string content)
    => await GetTools().CreateProjectFile(projectId, relativePath, content).ConfigureAwait(false);

    [McpTool(Cx.UpdateFileCmd, Cx.UpdateFileDesc)]
    public static async Task<object> UpdateProjectFile(
    [Description(Cx.ProjectParamDesc)] int projectId,
    [Description(Cx.FileSystemNodeIdParamDesc)] int fileSystemNodeId,
    [Description(Cx.FileContentParamDesc)] string content)
    => await GetTools().UpdateProjectFile(projectId, fileSystemNodeId, content).ConfigureAwait(false);


    [McpTool(Cx.CreateFolderCmd, Cx.CreateFolderDesc)]
    public static async Task<object> CreateProjectDirectory(
    [Description(Cx.ProjectParamDesc)] int projectId,
    [Description(Cx.FolderPathParamDesc)] string path)
    => await GetTools().CreateFolderAsync(projectId, path).ConfigureAwait(false);

  }
}
