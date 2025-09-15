using DaemonsMCP.Core.Config;
using DaemonsMCP.Core.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using Serilog.Core;
using System.IO;

namespace DaemonsMCP.Tests.Config
{
    [TestClass]
    public class AppConfigTests
    {
        private string _testConfigPath;
        private string _testConfigFile;
        private readonly ILoggerFactory _loggerFactory;

        
        public AppConfigTests()
        {
            _testConfigPath = Path.Combine(Path.GetTempPath(), "DaemonsMCPTests", Guid.NewGuid().ToString());
            _testConfigFile = Path.Combine(_testConfigPath, "daemonsmcp.json");
            Directory.CreateDirectory(_testConfigPath);
            File.Copy("daemonsmcp.json", _testConfigFile, true); // Assuming the test config file is in the project root

            var testLogsPath = Path.Combine(Path.GetTempPath(), "DaemonsMCP-Tests", "logs");
            Directory.CreateDirectory(testLogsPath);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Warning)) // Reduce Microsoft logging noise
                .MinimumLevel.Override("System", new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Warning))
                .Enrich.FromLogContext()
                .Enrich.FromLogContext()
                .WriteTo.File(
                    path: Path.Combine(testLogsPath, "test-indexservice-.log"),
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
                )
                .WriteTo.Debug() // Shows in Visual Studio Debug Output window
                .CreateLogger();

            _loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(Log.Logger));

          Log.Information("=== IndexServiceTests started ===");
          Log.Information("Test logs will be written to: {LogPath}", testLogsPath);
        }


       [TestCleanup]
        public void TestCleanup()
        {
            if (Directory.Exists(_testConfigPath))
            {
                Directory.Delete(_testConfigPath, true);
            }
            Log.Information("=== IndexServiceTests completed ===");
            Log.CloseAndFlush();
            _loggerFactory?.Dispose();
        }

        [TestMethod]
        public void Constructor_WithValidConfigPath_ShouldLoadConfiguration()
        {            
            var config = new AppConfig(_loggerFactory, _testConfigPath);

            config.Should().NotBeNull();
            config.IsConfigured.Should().BeTrue();
            config.ConfigPath.Should().Be(_testConfigFile);
            config.Projects.Should().NotBeNull();
            config.Projects.Count.Should().BeGreaterThan(0);
    }

        [TestMethod]
        public void Constructor_WithInvalidConfigPath_ShouldUseDefaults()
        {            
            var invalidPath = Path.Combine(_testConfigPath, "invalid");
            var config = new AppConfig(_loggerFactory, invalidPath);

           config.Should().NotBeNull();
           config.IsConfigured.Should().BeFalse();
           config.ConfigPath.Should().BeNull();
           config.Projects.Should().NotBeNull();
           config.Projects.Count.Should().Be(0);

        }
            

        [TestMethod]
        public void LoadConfiguration_WithValidJsonFile_ShouldReturnConfiguration()
        {
            // TODO: Implement test for valid JSON configuration loading
        }

        [TestMethod]
        public void LoadConfiguration_WithInvalidJsonFile_ShouldReturnNull()
        {
            // TODO: Implement test for invalid JSON configuration handling
        }

        [TestMethod]
        public void LoadConfiguration_WithMissingFile_ShouldReturnNull()
        {
            // TODO: Implement test for missing configuration file
        }

        [TestMethod]
        public void LoadConfiguration_WithEmptyFile_ShouldReturnNull()
        {
            // TODO: Implement test for empty configuration file
        }

        [TestMethod]
        public void LoadProjectsFromConfig_WithValidProjects_ShouldReturnProjectDictionary()
        {
            // TODO: Implement test for valid project loading
        }

        [TestMethod]
        public void LoadProjectsFromConfig_WithDisabledProjects_ShouldExcludeDisabled()
        {
            // TODO: Implement test for disabled project filtering
        }

        [TestMethod]
        public void LoadProjectsFromConfig_WithInvalidProjectPaths_ShouldSkipInvalid()
        {
            // TODO: Implement test for invalid project path handling
        }

        [TestMethod]
        public void LoadProjectsFromConfig_WithNonExistentPaths_ShouldSkipMissing()
        {
            // TODO: Implement test for non-existent project path handling
        }

        [TestMethod]
        public void LoadProjectsFromConfig_WithDuplicateNames_ShouldHandleCorrectly()
        {
            // TODO: Implement test for duplicate project name handling
        }

        [TestMethod]
        public void GetConfigurationPaths_ShouldReturnCorrectOrderedPaths()
        {
            // TODO: Implement test for configuration path discovery order
        }

        [TestMethod]
        public void IsConfigured_WithLoadedConfig_ShouldReturnTrue()
        {
            // TODO: Implement test for configuration status check
        }

        [TestMethod]
        public void IsConfigured_WithoutConfig_ShouldReturnFalse()
        {
            // TODO: Implement test for unconfigured status check
        }

        [TestMethod]
        public void Projects_WithLoadedProjects_ShouldReturnReadOnlyDictionary()
        {
            // TODO: Implement test for projects property access
        }

        [TestMethod]
        public void Security_WithLoadedConfig_ShouldReturnSecuritySettings()
        {
            // TODO: Implement test for security settings access
        }

        [TestMethod]
        public void Security_WithoutConfig_ShouldReturnDefaults()
        {
            // TODO: Implement test for default security settings
        }

        [TestMethod]
        public void Version_WithLoadedConfig_ShouldReturnVersionSettings()
        {
            // TODO: Implement test for version settings access
        }

        [TestMethod]
        public void Version_WithoutConfig_ShouldReturnDefaults()
        {
            // TODO: Implement test for default version settings
        }

        [TestMethod]
        public void Reload_WithNewConfigPath_ShouldReloadConfiguration()
        {
            // TODO: Implement test for configuration reloading
        }

        [TestMethod]
        public void Reload_WithInvalidPath_ShouldUseDefaults()
        {
            // TODO: Implement test for reload with invalid path
        }

        [TestMethod]
        public void ConfigPath_AfterSuccessfulLoad_ShouldReturnLoadedPath()
        {
            // TODO: Implement test for config path tracking
        }

        [TestMethod]
        public void ResolvePath_WithRelativePath_ShouldReturnAbsolutePath()
        {
            // TODO: Implement test for relative path resolution
        }

        [TestMethod]
        public void ResolvePath_WithAbsolutePath_ShouldReturnUnchanged()
        {
            // TODO: Implement test for absolute path handling
        }
    }
}