using DaemonsMCP.Core.Config;
using MCPSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core {
  public class DaemonsMcpHostedService(IServiceProvider serviceProvider, ILogger<DaemonsMcpHostedService> logger) : BackgroundService {
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<DaemonsMcpHostedService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
      _logger.LogInformation("DaemonsMCP Service starting...");

      try {
        // Validate configuration
        using var scope = _serviceProvider.CreateScope();
        var config = scope.ServiceProvider.GetRequiredService<IAppConfig>();

        if (!config.Projects.Any()) {
          _logger.LogError("No projects configured. Service cannot start.");
          throw new InvalidOperationException("No projects configured");
        }

        _logger.LogInformation("Starting with {ProjectCount} projects", config.Projects.Count);

        // Initialize the DI bridge
        DIServiceBridge.Initialize(_serviceProvider);
        _logger.LogDebug("Initialized DI service bridge");

        // Register the bridge with MCPSharp (it has parameterless constructor)
        MCPServer.Register<DaemonsToolsBridge>();
        _logger.LogInformation("Registered DaemonsToolsBridge with MCPSharp server");

        // Start MCPSharp server - this will block until cancellation
        await MCPServer.StartAsync("DaemonsMCP", "2.0.0");

        _logger.LogInformation("DaemonsMCP Service completed");
      } catch (OperationCanceledException) {
        _logger.LogInformation("DaemonsMCP Service stopped gracefully");
      } catch (Exception ex) {
        _logger.LogError(ex, "DaemonsMCP Service encountered an error");
        throw;
      }
    }
  }

}
