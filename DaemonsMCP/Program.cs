using System.Text.Json;
using System.Text.Json.Serialization;

namespace DaemonsMCP
{
    
    static public class Program
    {
        private static IReadOnlyDictionary<string, Project> _projects = null!;

        // Add property to access projects from other classes
        public static IReadOnlyDictionary<string, Project> Projects => _projects;

        static async Task Main()    
        {
            // One-time initialization
            GlobalConfig.Initialize();

            if (!GlobalConfig.Projects.Any()) {
              await Console.Error.WriteLineAsync("[DaemonsMCP] No projects configured.");
              return;
            }

            await Console.Error.WriteLineAsync($"[DaemonsMCP] Starting with {GlobalConfig.Projects.Count} projects");


            while (true)
            {
                string? input = await Console.In.ReadLineAsync().ConfigureAwait(false);
                if (string.IsNullOrEmpty(input)) break;           
                JsonRpcInitResponse? response;
                try
                {                    
                    var request = JsonSerializer.Deserialize<JsonRpcRequest>(input);
                    if (request == null || request.JsonRpc != "2.0")
                    {
                       ToolsHandler.SendErrorResponse(null, -32600, "[DaemonsMCP] Invalid Request");
                       continue;
                    }
                    
                    await Console.Error.WriteLineAsync($"[DaemonsMCP] Received method: {input}").ConfigureAwait(false);                    
                    // Handle MCP protocol methods
                    response = request.Method switch
                    {
                        Tx.Initialize => HandleInitialize(request),
                        Tx.InitializedNotification => null, // No response for notifications
                        Tx.ListMethods => ToolsHandler.HandleToolsList(request),
                        Tx.CallMethod => await ToolsHandler.HandleToolsCall(request).ConfigureAwait(false),
                        _ => new JsonRpcResponse
                        {                           
                           Error = new { code = -32601, message = "[DaemonsMCP] Method not found" },
                           Id = request.Id
                        }
                    };
              
                    string responseJson = JsonSerializer.Serialize(response);
                    await Console.Out.WriteLineAsync(responseJson).ConfigureAwait(false);
                    await Console.Out.FlushAsync().ConfigureAwait(false);
                    await Console.Error.WriteLineAsync($"[DaemonsMCP][LOG] Serialized response: {responseJson}").ConfigureAwait(false);
                }
                catch (JsonException je)
                {                    
                    await Console.Error.WriteLineAsync($"[DaemonsMCP][Error1]: {je.Message}").ConfigureAwait(false);
                }
                 catch (Exception ex)
                {
                    await Console.Error.WriteLineAsync($"[DaemonsMCP][Error2]: {ex.Message}").ConfigureAwait(false);
                }


            }
        }

         private static JsonRpcInitResponse HandleInitialize(JsonRpcRequest request)
         {
            return new JsonRpcInitResponse
            {
                JsonRpc = "2.0",
                Result = new
                {
                    protocolVersion = "2025-06-18",
                    capabilities = new
                    {
                        tools = new { }
                    },
                    serverInfo = new
                    {
                        name = "DaemonsMCP",
                        version = "1.0.0"
                    }
                },
                Id = request.Id
            };
         }

    }
}