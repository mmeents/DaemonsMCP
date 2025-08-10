using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Extensions {
  public static class Cx {
    public const string AppName = "DaemonsMCP";
    public const string AppVersion = "0.1.0";
    public const string CONFIG_FILE_NAME = "daemonsmcp.json";
    public const bool IsDebug = false;

    // Tools Commands    
    public const string ListMethodsCmd = "tools/list";
    public const string CallMethodCmd = "tools/call";
    public const string InitializeCmd = "initialize";
    public const string InitializedNotificationCmd = "notifications/initialized";
    public const string ListResourcesCmd = "resources/list";
    public const string PromptsListCmd = "prompts/list";
    

    // Tools Names
    public const string ListProjectsCmd = "local-list-projects";
    public const string ListFoldersCmd = "local-list-project-directories";
    public const string ListFilesCmd = "local-list-project-files";

    public const string GetFileCmd = "local-get-project-file";
    public const string InsertFileCmd = "local-create-project-file";
    public const string UpdateFileCmd = "local-update-project-file";
    public const string DeleteFileCmd = "local-delete-project-file";

    public const string CreateFolderCmd = "local-create-project-directory";
    public const string DeleteFolderCmd = "local-delete-project-directory";

    // Tool descriptions
    public const string ListProjectsDesc = "Gets list of available projects. A project is the name of the root folder.";
    public const string ListFoldersDesc = "local-list-project-directories";
    public const string ListFilesDesc = "local-list-project-files";

    public const string GetFileDesc = "local-get-project-file";
    public const string InsertFileDesc = "local-create-project-file";
    public const string UpdateFileDesc = "local-update-project-file";
    public const string DeleteFileDesc = "local-delete-project-file";

    public const string CreateFolderDesc = "local-create-project-directory";
    public const string DeleteFolderDesc = "local-delete-project-directory";

    // Tool parameter descriptions
    public const string ProjectNameParamDesc = $"Project name from {Cx.ListProjectsCmd}";
    public const string FolderPathParamDesc = "Path to the directory. (relative to project root) If empty, the root of the project is used.";
    public const string FolderFilterParamDesc = "Filter for directories. If empty, no filter is applied.";
    public const string FilePathParamDesc = "Path to the file. (relative to project root)";
    public const string FileFilterParamDesc = "Filter for files. If empty, no filter is applied.";

    public const string FileContentParamDesc = "File content to write";
    public const string CreateFolderOptionDesc = "Create folders if it they don't exist";
    public const string OverwriteFileOptionDesc = "Overwrite file if it already exists";

    // Debugging prefixes
    public const string Dd0 = "[DaemonsMCP][Debug]";
    public const string Dd1 = "[DaemonsMCP][ProjectTool] ";
    public const string Dd2 = "[DaemonsMCP][Security] ";

    // Error Messages
    public const string InvalidSkippingProject = "Skipping invalid project configuration";
    public const string ErrorNoConfig = "AppConfig cannot be null";

  }
}
