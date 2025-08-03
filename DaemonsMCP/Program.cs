using MCPSharp;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DaemonsMCP
{
  static class Program {
    static async Task Main() {
      // Initialize configuration
      GlobalConfig.Initialize();

      if (!GlobalConfig.Projects.Any()) {
        await Console.Error.WriteLineAsync("[DaemonsMCP] No projects configured.");
        return;
      }

      await Console.Error.WriteLineAsync($"[DaemonsMCP] Starting with {GlobalConfig.Projects.Count} projects");

      // Register tools
      MCPServer.Register<ProjectTools>();

      // Start the MCPSharp server
      await MCPServer.StartAsync("DaemonsMCP", "1.0.0");
    }
  }

}