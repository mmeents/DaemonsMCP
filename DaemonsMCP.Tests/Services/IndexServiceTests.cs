using DaemonsMCP.Core.Config;
using DaemonsMCP.Core.Repositories;
using DaemonsMCP.Core.Services;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Tests.Services {
  [TestClass]
  public class IndexServiceTests {
    private static ILoggerFactory _loggerFactory = null!;
    private readonly IAppConfig _config;
    private readonly IValidationService _validationService;
    private readonly ISecurityService _securityService;
    private readonly IIndexRepository _indexRepository;
    private readonly IIndexService _indexService;
    public IndexServiceTests() {
      var testLogsPath = Path.Combine(Path.GetTempPath(), "DaemonsMCP-Tests", "logs");
      Directory.CreateDirectory(testLogsPath);

      Log.Logger = new LoggerConfiguration()
          .MinimumLevel.Debug()
          .MinimumLevel.Override("Microsoft", new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Warning)) // Reduce Microsoft logging noise
          .MinimumLevel.Override("System", new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Warning))
          .Enrich.FromLogContext()
          .WriteTo.File(
              path: Path.Combine(testLogsPath, "test-indexservice-.log"),
              rollingInterval: RollingInterval.Day,
              outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
          )
          .WriteTo.Debug() // Shows in Visual Studio Debug Output window
          .CreateLogger();

      _loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(Log.Logger));

      Log.Information("🚀 === IndexServiceTests started ===");
      Log.Information("Test logs will be written to: {LogPath}", testLogsPath);

      _config = new AppConfig(_loggerFactory);        
      _securityService = new SecurityService(_loggerFactory, _config);
      _validationService = new ValidationService(_config, _securityService);
      _indexRepository = new IndexRepository(_loggerFactory, _config, _validationService);
      _indexService = new IndexService(_loggerFactory, _config, _validationService, _securityService, _indexRepository);

    }

    [ClassCleanup]
    public static void ClassTeardown() {
      _loggerFactory?.Dispose();

      Log.Information("=== IndexServiceTests completed ===");
      Log.CloseAndFlush();
    }

    [TestMethod]
    public async Task RebuildIndex_ShouldRebuildIndexSuccessfully() {
          
      // Act
      var result = await _indexService.RebuildIndexAsync(true);
      // Assert
      Assert.IsTrue(result.Success, "Index rebuild should succeed");
      _indexService?.Dispose();


    }



  }
}
