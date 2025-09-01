using DaemonsMCP.Core;
using DaemonsMCP.Core.Config;
using DaemonsMCP.Core.Repositories;
using DaemonsMCP.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;

namespace DaemonsMCP
{
  class Program {
    static async Task Main(string[] args) {
      // Configure Serilog BEFORE creating the host
      ConfigureSerilog();

      try {
        var host = CreateHostBuilder(args).Build();
        await host.RunAsync();
      } catch (Exception ex) {
        Log.Fatal(ex, "Application terminated unexpectedly");
      } finally {
        Log.CloseAndFlush();
      }
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) => {
              // Configuration
              services.AddSingleton<IAppConfig, AppConfig>();
              services.AddSingleton<ISecurityService, SecurityService>();
              services.AddSingleton<IValidationService, ValidationService>();              
              services.AddSingleton<IIndexRepository, IndexRepository>();
              services.AddSingleton<IItemRepository, ItemRepository>();

              // Core services
              services.AddScoped<IProjectsService, ProjectService>();
              services.AddScoped<IProjectFolderService, ProjectFolderService>();
              services.AddScoped<IProjectFileService, ProjectFileService>();              
              services.AddScoped<IIndexService, IndexService>();
              services.AddScoped<IClassService, ClassService>();

              // MCP Tools (injected with dependencies)
              services.AddScoped<DaemonsTools>();

              // Hosted service for MCP protocol
              services.AddHostedService<DaemonsMcpHostedService>();
            });

    private static void ConfigureSerilog() {
      // Ensure logs directory exists
      var logsPath = Path.Combine(AppContext.BaseDirectory, "logs");
      Directory.CreateDirectory(logsPath);

      Log.Logger = new LoggerConfiguration()
          .MinimumLevel.Warning() // Adjust as needed
          .MinimumLevel.Override("Microsoft", new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Warning)) // Reduce Microsoft logging noise
          .MinimumLevel.Override("System", new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Warning))
          .Enrich.FromLogContext()
          .WriteTo.File(
              path: Path.Combine(logsPath, "daemons-.log"),
              rollingInterval: RollingInterval.Day,
              retainedFileCountLimit: 7,
              outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
          )
          .WriteTo.File(
              path: Path.Combine(logsPath, "daemons-errors-.log"),
              rollingInterval: RollingInterval.Day,
              retainedFileCountLimit: 30,
              restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning, // Only warnings and errors
              outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
          )
          .CreateLogger();

      Log.Information("Serilog configured - logging to {LogsPath}", logsPath);
    }

  }

}