using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;

namespace DaemonsMCP
{
    public static class Px { 
        public const string listProjects = "local_list_projects";
        public const string ProjectDescription  = "Gets list of available projects. A project is the name of the root folder.";

        public const string listProjectDirectory = "local_list_project_directories";
        public const string ProjectDirectoryDescription = "Gets list of directories in the project. The project name is required and must match the current project.";

        public const string listProjectFiles = "local_list_project_files";
        public const string ProjectFilesDescription = "Gets list of files in the project. The project name is required and must match the current project.";

        public const string getProjectFile = "local_get_project_file";
        public const string GetProjectFileDescription = "Gets the content of a file in the project. The project name is required and must match the current project. The path to the file is also required.";

        public const string projectNameParam = "projectName";
        public const string projectNameParamDesc = "Project name from list-projects";

        public const string pathParam = "path";
        public const string pathParamDesc = "Path to the directory or file. If empty, the root of the project is used.";

        public const string filterParam = "filter";
        public const string filterParamDesc = "Filter for files or directories. If empty, no filter is applied.";
    }
    public class ProjectHandler
    {
        private readonly Project _project;
        public ProjectHandler(Project Project) { 
          _project = Project;
          if (_project == null)
          {
              throw new ArgumentException("Project name cannot be null or empty", nameof(Project));
          }                    
        }

        public string ProjectName => _project.Name;
        public string ProjectPath => _project.Path;
        public string ProjectDescription => _project.Description;
        public static string MethodListProjects => Px.listProjects;
        public static string MethodListProjectDirectories => Px.listProjectDirectory;
        public static string MethodListProjectFiles => Px.listProjectFiles;
        public static string MethodGetProjectFile => Px.getProjectFile;

        public async Task<JsonRpcResponse> HandleRequest(JsonRpcRequest request){ 
            var response = new JsonRpcResponse()
            {                
                Id = request.Id,
                Result = null,
                Error = null
            };
            switch (request.Method) { 
                case Px.listProjects:
                    response.Result = new { projects = GlobalConfig.Projects.Select(p => new { p.Value.Name, p.Value.Description }) };
                    break;
                case Px.listProjectDirectory :
                    if (request.Params == null || !request.Params.Value.TryGetProperty(Px.projectNameParam, out var projectName) || projectName.GetString() != ProjectName)
                    {
                        response.Error = new { code = -32602, 
                            message = $"[DaemonsMCP][Project] Invalid params:  {Px.projectNameParam}  is required and must match a project.." };
                        break;                    
                    }
                    var path = request.Params.Value.TryGetProperty(Px.pathParam, out var p) ? p.GetString() : string.Empty;
                    var filter = request.Params.Value.TryGetProperty(Px.filterParam, out var f) ? f.GetString() : string.Empty;
                    if (string.IsNullOrEmpty(path)) {
                        path = ProjectPath; 
                    } else { 
                        path = Path.Combine(ProjectPath, path);
                    }
                    filter ??= string.Empty;
                    var folders = Directory.GetDirectories(path, filter, SearchOption.TopDirectoryOnly);
                    List<string> folderList = new List<string>();
                    foreach (var folder in folders){ 
                       var relativePath = folder.Substring(ProjectPath.Length).TrimStart(Path.DirectorySeparatorChar);
                       folderList.Add(relativePath);
                    }
                    response.Result = new { directories = folderList };
                    break;

                case Px.listProjectFiles:
                    
                    if (request.Params == null || !request.Params.Value.TryGetProperty(Px.projectNameParam, out var projectName2) || projectName2.GetString() != ProjectName)
                    {
                        response.Error = new { code = -32602, 
                            message = $"[DaemonsMCP][Project] Invalid params:  {Px.projectNameParam}  is required and must match the current project.." };
                        break;                    
                    }
                    var path2 = request.Params.Value.TryGetProperty(Px.pathParam, out var p2) ? p2.GetString() : string.Empty;
                    var filter2 = request.Params.Value.TryGetProperty(Px.filterParam, out var f2) ? f2.GetString() : string.Empty;
                    if (string.IsNullOrEmpty(path2)) {
                        path2 = ProjectPath; 
                    } else { 
                        path2 = Path.Combine(ProjectPath, path2);
                    }
                    filter2 ??= string.Empty;
                    var files = Directory.GetFiles(path2, filter2, SearchOption.TopDirectoryOnly);
                    List<string> fileList = new List<string>();
                    foreach (var file in files)
                    {
                        if (SecurityFilters.IsFileAllowed(file))
                        {
                            string relativePath = file.Substring(ProjectPath.Length).TrimStart(System.IO.Path.DirectorySeparatorChar);
                            fileList.Add(relativePath);
                        }                        
                    }
                    response.Result = new { files = fileList };
                    break;              
                case Px.getProjectFile :
                    if (request.Params == null || !request.Params.Value.TryGetProperty(Px.projectNameParam, out var projectName3) || projectName3.GetString() != ProjectName)
                    {
                        response.Error = new { code = -32602, 
                            message = $"[DaemonsMCP][Project] Invalid params: {Px.projectNameParam} is required and must match the current project." };
                        break;                    
                    }
                    var filePath = request.Params.Value.TryGetProperty("path", out var fPath) ? fPath.GetString() : string.Empty;
                    if (string.IsNullOrEmpty(filePath)) {
                        response.Error = new { code = -32602, message = $"[DaemonsMCP][Project] Invalid params: {Px.pathParam} is required." };
                        break;                    
                    }
                    var fullFilePath = System.IO.Path.Combine(ProjectPath, filePath);
                    if (!File.Exists(fullFilePath)) {
                        response.Error = new { code = -32602, message = $"[DaemonsMCP][Project] File not found: {fullFilePath}" };
                        break;                    
                    }
                    if (!SecurityFilters.IsFileAllowed(fullFilePath)) {
                        response.Error = new { code = -32602, 
                            message = "[DaemonsMCP][Project] Access to this file type is not allowed for security reasons."
                        };
                        break;
                    }
                    var fileInfo = new FileInfo(fullFilePath);
                    var reletivePath = fileInfo.FullName.Substring(ProjectPath.Length).TrimStart(System.IO.Path.DirectorySeparatorChar);                    
                    var fileExtension = fileInfo.Extension.ToLowerInvariant();
                    var fileEncoding = MimeTypesMap.DetectFileEncoding( fullFilePath);
                    string contentType = MimeTypesMap.GetMimeType(fileExtension);
                    bool isBinary = MimeTypesMap.IsBinaryFile(fullFilePath);
                    var fileContent = "";
                    if (!isBinary){ 
                        try
                        {
                            // Read the file content as text
                            fileContent = await File.ReadAllTextAsync(fullFilePath, Encoding.UTF8).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            response.Error = new { code = -32603, message = $"[DaemonsMCP][Project] Error reading file content: {ex.Message}" };
                            return response;
                        }
                    }
                    response.Result = new {
                        fileName= fileInfo.Name,
                        path= reletivePath, 
                        size= fileInfo.Length,
                        contentType,
                        encoding= fileEncoding, 
                        content= fileContent
                        };                    
                    break;

            }

            return response;
        }
    }
}
