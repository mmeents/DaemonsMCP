using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Application.ForWeb.GetReadme {
  public static class EndpointRegistry {
    public static List<EndpointDescription> GetAll() => new()
    {
        new EndpointDescription(
            "GET",
            "/api/readme",
            "Returns API documentation, available endpoints, projects, and nextToken for another request. Send token in request body as JSON.",
            new List<ParameterDescription>(){ 
              new ParameterDescription("token", "string", true, "One-time use authentication token")
            }            
        ),
        
        new EndpointDescription(
            "GET",
            "/api/files/search",
            "Searches the file system index for files and folders matching filters. Returns paginated results and nextToken. All parameters in request body as JSON.",
            new List<ParameterDescription>
            {
                new("token", "string", true, "One-time use authentication token"),
                new("projectId", "int", true, "The project to search in"),
                new("filter", "string", false, "Filter string - uses Contains matching on file/folder names"),
                new("includeFiles", "bool", false, "Include files in results (default: true)"),
                new("includeDirectories", "bool", false, "Include directories in results (default: true)"),
                new("pageNo", "int", false, "Page number, 1-based (default: 1)"),
                new("pageSize", "int", false, "Items per page (default: 20)")
            }
        ),

        new EndpointDescription(
            "GET",
            "/api/files/get",
            "Gets file contents by ProjectId and FileSystemNodeId. Returns file content and nextToken. Parameters in request body as JSON.",
            new List<ParameterDescription>
            {
                new("token", "string", true, "One-time use authentication token"),
                new("projectId", "int", true, "The project to which the file belongs"),
                new("fileSystemNodeId", "int", true, "The FileSystemNodeId of the file to retrieve")
            }            
        )       
        
    };
  }
}
