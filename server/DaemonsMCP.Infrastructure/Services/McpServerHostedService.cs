using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using MCPSharp;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaemonsMCP.Infrastructure.Extensions;
using DaemonsMCP.Infrastructure.Tools;
using DaemonsMCP.Domain.Constants;


namespace DaemonsMCP.Infrastructure.Services {
  public class McpServerHostedService : BackgroundService {
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<McpServerHostedService> _logger;

    public McpServerHostedService(
        IServiceProvider serviceProvider,
        ILogger<McpServerHostedService> logger) {
      _serviceProvider = serviceProvider;
      _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
      _logger.LogInformation("🚀 MCP Server starting");

      // Wait for other services to initialize
      await Task.Delay(3000, stoppingToken);

      // Initialize the DI bridge for MCPSharp
      DIServiceBridge.Initialize(_serviceProvider);

      // Register your tools bridge (parameterless constructor)
      MCPServer.Register<ProjectTools>();
      MCPServer.Register<FileSystemTools>();

      // Start MCPSharp server - blocks until cancellation
      await MCPServer.StartAsync(Cx.AppName, Cx.AppVersion);
    }
  }
}
