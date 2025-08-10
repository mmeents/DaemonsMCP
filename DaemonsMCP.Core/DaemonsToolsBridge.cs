using DaemonsMCP.Core.Extensions;
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

    // Projects 
    [McpTool(Cx.ListProjectsCmd, Cx.ListProjectsDesc)]
    public static async Task<object> ListProjects() => await GetTools().ListProjects();

    // Project Folders
    [McpTool(Cx.ListFoldersCmd, Cx.ListFoldersDesc)]
    public static async Task<object> ListProjectDirectories(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.FolderPathParamDesc)] string? path = null,
        [Description(Cx.FolderFilterParamDesc)] string? filter = null)
        => await GetTools().ListProjectDirectories(projectName, path, filter);

    [McpTool(Cx.CreateFolderCmd, Cx.CreateFolderDesc)]
    public static async Task<object> CreateProjectDirectory(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.FolderPathParamDesc)] string path,
        [Description(Cx.CreateFolderOptionDesc)] bool createDirectories = true)
        => await GetTools().CreateProjectDirectory(projectName, path, createDirectories);

    [McpTool(Cx.DeleteFolderCmd, Cx.DeleteFolderDesc)]
    public static async Task<object> DeleteProjectDirectory(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.FolderPathParamDesc)] string path,
        [Description(Cx.CreateFolderOptionDesc)] bool deleteDirectories = true)
        => await GetTools().DeleteProjectDirectory(projectName, path, deleteDirectories);

    // Project Files
    [McpTool(Cx.ListFilesCmd, Cx.ListFilesDesc)]
    public static async Task<object> ListProjectFiles(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.FilePathParamDesc)] string? path = null,
        [Description(Cx.FileFilterParamDesc)] string? filter = null)
        => await GetTools().ListProjectFiles(projectName, path, filter);

    [McpTool(Cx.GetFileCmd, Cx.GetFileDesc)]
    public static async Task<object> GetProjectFile(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.FilePathParamDesc)] string path)
        => await GetTools().GetProjectFile(projectName, path);

    [McpTool(Cx.InsertFileCmd, Cx.InsertFileDesc)]
    public static async Task<object> CreateProjectFile(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.FilePathParamDesc)] string path,
        [Description(Cx.FileContentParamDesc)] string content,
        [Description(Cx.CreateFolderOptionDesc)] bool createDirectories = true,
        [Description(Cx.OverwriteFileOptionDesc)] bool overwrite = false)
        => await GetTools().CreateProjectFile(projectName, path, content, createDirectories, overwrite);

    [McpTool(Cx.UpdateFileCmd, Cx.UpdateFileDesc)]
    public static async Task<object> UpdateProjectFile(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.FilePathParamDesc)] string path,
        [Description(Cx.FileContentParamDesc)] string content,
        [Description(Cx.CreateFolderOptionDesc)] bool createBackup = true)
        => await GetTools().UpdateProjectFile(projectName, path, content, createBackup);

    [McpTool(Cx.DeleteFileCmd, Cx.DeleteFileDesc)]
    public static async Task<object> DeleteProjectFile(
        [Description(Cx.ProjectNameParamDesc)] string projectName,
        [Description(Cx.FilePathParamDesc)] string path)
        => await GetTools().DeleteProjectFile(projectName, path);
  }
}
