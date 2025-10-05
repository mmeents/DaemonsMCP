using DaemonsMCP.Core.Extensions;
using DaemonsMCP.Core.Repositories;
using DaemonsMCP.Core.Services;
using DaemonsMCP.Core.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;

namespace DaemonsConfigViewer {
  public class DaemonsAppContext : ApplicationContext {
    private readonly ServiceProvider _serviceProvider;
    private readonly ILogger<DaemonsAppContext> _logger;
    private Form1? form1 = null;

    public DaemonsAppContext() {
      // Build service provider     
      _serviceProvider = BuildServiceProvider();
      _logger = _serviceProvider.GetRequiredService<ILogger<DaemonsAppContext>>();

      _logger.LogInformation("🚀 DaemonsAppContext initialized");

      // Setup cleanup on application exit
      Application.ApplicationExit += OnApplicationExit;
    }   

    private ServiceProvider BuildServiceProvider() {
      var logsPath = Sx.LogsAppPath;
      Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Warning)) // Reduce Microsoft logging noise
        .MinimumLevel.Override("System", new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Warning))
        .Enrich.FromLogContext()
        .WriteTo.File(
            path: Path.Combine(logsPath, "DaemonsConfigViewer-.log"),
            rollingInterval: RollingInterval.Day,
            outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
        )
        .WriteTo.Debug() // Shows in Visual Studio Debug Output window
        .CreateLogger();

      var services = new ServiceCollection();

      // Logging
      services.AddLogging(builder => builder.AddSerilog(Log.Logger));
      services.AddConfigViewer();

      return services.BuildServiceProvider();
    } 

    public T GetService<T>() where T : notnull {
      return _serviceProvider.GetRequiredService<T>();
    }

    public Form CreateMainForm() {
      if (form1 == null) {
        form1 = new Form1(_serviceProvider);
      }      
      return form1;
    }

    private bool _disposed = false;
    private void OnApplicationExit(object? sender, EventArgs e) {
      if (_disposed) return; // ← Guard against double disposal
      _disposed = true;
      _logger.LogInformation("🛑 Application shutting down, final disposing resources...");
      // Unsubscribe
      Application.ApplicationExit -= OnApplicationExit;
            
      form1?.Dispose();      
      if (_serviceProvider is IDisposable disposable) {
        disposable.Dispose();  // Dispose service provider (cascades to all IDisposable singletons)
      }
      // Close Serilog
      Log.CloseAndFlush();
    }

    protected override void Dispose(bool disposing) {
      if (disposing) {
        _logger?.LogInformation("🧹 DaemonsAppContext disposing");
        OnApplicationExit(null, EventArgs.Empty);
      }
      base.Dispose(disposing);
    }

  }
}
