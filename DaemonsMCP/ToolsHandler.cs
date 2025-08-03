using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DaemonsMCP
{
    public static class Tx {
        public const string ListMethods = "tools/list";
        public const string CallMethod = "tools/call";
        public const string Initialize = "initialize";
        public const string InitializedNotification = "notifications/initialized"; 
        public const string ListResources = "resources/list";
        public const string PromptsList = "prompts/list";
    }

   static class ToolsHandler
   {  
       public static async Task<JsonRpcResponse> HandleRequest(JsonRpcRequest request)
       {
           return request.Method switch
           {
               Tx.ListMethods => HandleToolsList(request),
               Tx.CallMethod => await HandleToolsCall(request).ConfigureAwait(false),
               _ => new JsonRpcResponse
               {
                   Error = new { code = -32601, message = "Method not found" },
                   Id = request.Id
               }
           };
       }
    
       public static JsonRpcResponse HandleToolsList(JsonRpcRequest request)
       {        
           return new JsonRpcResponse
           {               
               Result = new { tools = new List<McpTool>(){ 
                   new McpTool(){ 
                       Name = Px.listProjects,
                       Description = Px.ProjectDescription,                       
                   }, // Px.listProjects
                   new McpTool(){ 
                       Name = Px.listProjectDirectory,
                       Description = Px.ProjectDirectoryDescription,
                       InputSchema = new InputSchema
                       {                           
                           Properties = new Dictionary<string, InputSchemaProperty>
                           {
                               { Px.projectNameParam, new InputSchemaProperty {  Description = Px.projectNameParamDesc } },
                               { Px.pathParam, new InputSchemaProperty {  Description = Px.pathParamDesc } },
                               { Px.filterParam, new InputSchemaProperty { Description = Px.filterParamDesc } }
                           },
                           Required = [Px.projectNameParam] 
                       }
                   }, // Px.listProjectDirectory
                   new McpTool(){ Name = Px.listProjectFiles,
                       Description = Px.ProjectFilesDescription,
                       InputSchema = new InputSchema
                       {                           
                           Properties = new Dictionary<string, InputSchemaProperty>
                           {
                               { Px.projectNameParam, new InputSchemaProperty { Description = Px.projectNameParamDesc } },
                               { Px.pathParam, new InputSchemaProperty { Description = Px.pathParamDesc } },
                               { Px.filterParam, new InputSchemaProperty { Description = Px.filterParamDesc } }
                           },
                           Required = [Px.projectNameParam]
                       }
                   }, // Px.listProjectFiles
                   new McpTool(){ 
                       Name = Px.getProjectFile,
                       Description = Px.GetProjectFileDescription,
                       InputSchema = new InputSchema
                       {                           
                           Properties = new Dictionary<string, InputSchemaProperty>
                           {
                               { Px.projectNameParam, new InputSchemaProperty { Description = Px.projectNameParamDesc } },
                               { Px.pathParam, new InputSchemaProperty { Description = Px.pathParamDesc } }
                           },
                           Required = [Px.projectNameParam, Px.pathParam]
                       }
                   }  // Px.getProjectFile
               } },
               Id = request.Id
           };
       }

       public static async Task<JsonRpcResponse> HandleToolsCall(JsonRpcRequest request)
       {
           try
           {
               if (request.Params == null || 
                   !request.Params.Value.TryGetProperty("name", out var toolName) ||
                   !request.Params.Value.TryGetProperty("arguments", out var arguments))
               {
                   return new JsonRpcResponse
                   {                       
                       Error = new { code = -32602, message = "[DaemonsMCP][Tools] Invalid params: name and arguments required" },
                       Id = request.Id
                   };
               }

               var tool = toolName.GetString();
            
               return tool switch
               {
                   Px.listProjects => HandleListProjects(request),
                   Px.listProjectDirectory => await HandleListProjectDirectories(request, arguments).ConfigureAwait(false),
                   Px.listProjectFiles => await HandleListProjectFiles(request, arguments).ConfigureAwait(false),
                   Px.getProjectFile => await HandleGetProjectFile(request, arguments).ConfigureAwait(false),
                   _ => new JsonRpcResponse
                   {
                      JsonRpc = "2.0",
                       Error = new { code = -32601, message = $"[DaemonsMCP][Tools] Tool {tool} not supported." },
                       Id = request.Id
                   }
               };
           }
           catch (Exception ex)
           {
               return new JsonRpcResponse
               {
                   Error = new { code = -32603, message = $"[DaemonsMCP][Tools] Internal error: {ex.Message}" },
                   Id = request.Id
               };
           }
       }

       private static JsonRpcResponse HandleListProjects(JsonRpcRequest request)
       {           
           return new JsonRpcResponse
           {
               Result = new { projects = GlobalConfig.Projects.Select(p => new { p.Value.Name, p.Value.Description }) },
               Id = request.Id
           };
       }

       private static async Task<JsonRpcResponse> HandleListProjectDirectories(JsonRpcRequest request, JsonElement arguments)
       {
           if (!arguments.TryGetProperty(Px.projectNameParam, out var projectNameElement))
           {
               return new JsonRpcResponse
               {                   
                   Error = new { code = -32602, message = "[DaemonsMCP][Tools] projectName is required" },
                   Id = request.Id
               };
           }
           var projectName = projectNameElement.GetString();
           
           if (string.IsNullOrEmpty(projectName) || !GlobalConfig.Projects.TryGetValue(projectName, out Project? project))
           {
               return new JsonRpcResponse
               {
                   Error = new { code = -32602, message = $"[DaemonsMCP][Tools] Invalid {Px.projectNameParam}" },
                   Id = request.Id
               };
           }
           var projectHandler = new ProjectHandler(project);
        
           // Create a fake request for the project handler
           var projectRequest = new JsonRpcRequest
           {
               Method = Px.listProjectDirectory,
               Params = arguments,
               Id = request.Id
           };

           return await projectHandler.HandleRequest(projectRequest).ConfigureAwait(false);
       }

       private static async Task<JsonRpcResponse> HandleListProjectFiles(JsonRpcRequest request, JsonElement arguments)
       {
           if (!arguments.TryGetProperty(Px.projectNameParam, out var projectNameElement))
           {
               return new JsonRpcResponse
               {                   
                   Error = new { code = -32602, message = $"[DaemonsMCP][Tools] {Px.projectNameParam} is required" },
                   Id = request.Id
               };
           }

           var projectName = projectNameElement.GetString();           
           if (string.IsNullOrEmpty(projectName) || !GlobalConfig.Projects.ContainsKey(projectName))
           {
               return new JsonRpcResponse
               {                   
                   Error = new { code = -32602, message = $"[DaemonsMCP][Tools] Invalid {Px.projectNameParam} : {projectName}" },
                   Id = request.Id
               };
           }

           var project = GlobalConfig.Projects[projectName];
           var projectHandler = new ProjectHandler(project);
       
           var projectRequest = new JsonRpcRequest
           {
               Method = Px.listProjectFiles,
               Params = arguments,
               Id = request.Id
           };
           return await projectHandler.HandleRequest(projectRequest).ConfigureAwait(false);
       }

       private static async Task<JsonRpcResponse> HandleGetProjectFile(JsonRpcRequest request, JsonElement arguments)
        {
            if (!arguments.TryGetProperty(Px.projectNameParam, out var projectNameElement))
            {
                return new JsonRpcResponse
                {                 
                    Error = new { code = -32602, message = $"{Px.projectNameParam} is required" },
                    Id = request.Id
                };
            }

            var projectName = projectNameElement.GetString();

            if (string.IsNullOrEmpty(projectName) || !GlobalConfig.Projects.ContainsKey(projectName))
            {
                return new JsonRpcResponse
                {                
                    Error = new { code = -32602, message = $"[DaemonsMCP][Tools] Invalid projectName {projectName}" },
                    Id = request.Id
                };
            }

            var project = GlobalConfig.Projects[projectName];
            var projectHandler = new ProjectHandler(project);
        
            var projectRequest = new JsonRpcRequest
            {
                Method = Px.getProjectFile, 
                Params = arguments,
                Id = request.Id
            };

            return await projectHandler.HandleRequest(projectRequest).ConfigureAwait(false);
        }

        public static JsonRpcResponse SendErrorResponse(JsonElement? id, int code, string message)
        {
            return new JsonRpcResponse
            {                
                Error = new { code, message },
                Id = id
            };
        }
    
  }

    public class McpTool
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
    
        [JsonPropertyName("description")]
        public string Description { get; set; } = "";
    
        [JsonPropertyName("inputSchema")]
        public InputSchema InputSchema { get; set; } = new InputSchema();
    }

    public class InputSchema
    { 
        [JsonPropertyName("type")]
        public string Type { get; set; } = "object";
    
        [JsonPropertyName("properties")]
        public Dictionary<string, InputSchemaProperty> Properties { get; set; } = new Dictionary<string, InputSchemaProperty>();

        [JsonPropertyName("required")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string[]? Required { get; set; } = null;
        }

    public class InputSchemaProperty
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "string";
    
        [JsonPropertyName("description")]
        public string Description { get; set; } = "";
    }


}
