using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DaemonsMCP.Core.Config;
using DaemonsMCP.Core.Services;
using DaemonsMCP.Core;

namespace DaemonsMCP
{
  class Program {
    static async Task Main(string[] args) {
      var host = CreateHostBuilder(args).Build();

      await host.RunAsync();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) => {
              // Configuration
              services.AddSingleton<IAppConfig, AppConfig>();

              // Core services
              services.AddScoped<IValidationService, ValidationService>();
              services.AddScoped<ISecurityService, SecurityService>();
              services.AddScoped<IProjectsService, ProjectService>();
              services.AddScoped<IProjectFolderService, ProjectFolderService>();
              services.AddScoped<IProjectFileService, ProjectFileService>();

              // MCP Tools (injected with dependencies)
              services.AddScoped<DaemonsTools>();

              // Hosted service for MCP protocol
              services.AddHostedService<DaemonsMcpHostedService>();
            })
            .ConfigureLogging(logging => {
              logging.ClearProviders();
              logging.AddConsole();
              logging.SetMinimumLevel(LogLevel.Error);
            });
  }

}