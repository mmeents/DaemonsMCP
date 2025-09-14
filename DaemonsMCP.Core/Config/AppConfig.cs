using DaemonsMCP.Core.Models;
using DaemonsMCP.Core.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PackedTables.Net;
using Microsoft.Extensions.Logging;

namespace DaemonsMCP.Core.Config 
{
  public class AppConfig : IAppConfig {
    private readonly ILogger<AppConfig> _logger;
    public AppConfig(ILoggerFactory loggerFactory, string? configPath = null) {
      if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
      _logger = loggerFactory.CreateLogger<AppConfig>();
      if (Cx.IsDebug) { 
        _logger.LogDebug($"{Cx.Dd0} Initializing AppConfig with config path: {configPath ?? "default"}");
        _logger.LogDebug($"{Cx.Dd0} Current Directory: {Directory.GetCurrentDirectory()}");
        _logger.LogDebug($"{Cx.Dd0} Executable Directory: {AppContext.BaseDirectory}");
        _logger.LogDebug($"{Cx.Dd0} Process MethodName: {System.Diagnostics.Process.GetCurrentProcess().ProcessName}");
      }
      _config = LoadConfiguration(configPath);
      _projects = LoadProjectsFromConfig();
      var projectCount = _projects?.Count ?? 0;
      if (Cx.IsDebug) _logger.LogDebug($"{Cx.Dd0} Initialized with {projectCount} projects");
    }
    private DaemonsMcpConfiguration? _config = null;
    private Dictionary<string, DaemonsMCP.Core.Models.ProjectModel>? _projects;
    
    public bool IsConfigured => _config != null;
    public string? ConfigPath { get; private set;}
    public VersionSettings Version => _config?.Version ?? new VersionSettings();
    public SecuritySettings Security => _config?.Security ?? new SecuritySettings();    
    public IReadOnlyDictionary<string, ProjectModel> Projects => _projects ?? [];


    private DaemonsMcpConfiguration? LoadConfiguration(string? path = null) {
      string configFileName = Cx.CONFIG_FILE_NAME;
      if (path != null) {
        configFileName = Path.GetFullPath(Path.Combine(path, Cx.CONFIG_FILE_NAME));
      }
      var configPaths = path == null ? GetConfigurationPaths() : [configFileName];

      foreach (var configPath in configPaths) {
        if (!File.Exists(configPath)) continue;

        try {
          var jsonContent = File.ReadAllText(configPath);
          var config = JsonSerializer.Deserialize<DaemonsMcpConfiguration>(jsonContent, Sx.DefaultJsonOptions);

          if (config != null) {
            if (Cx.IsDebug) _logger.LogDebug($"{Cx.Dd0} Loaded configuration from: {configPath}");
            ConfigPath = configPath; // Store the path for later use
            return config;
          }
        } catch (JsonException ex) {
          if (Cx.IsDebug) _logger.LogDebug($"{Cx.Dd0} Invalid JSON in {configPath}: {ex.Message}");
        } catch (Exception ex) {
          if (Cx.IsDebug) _logger.LogDebug($"{Cx.Dd0} Error reading {configPath}: {ex.Message}");
        }
      }

      return null;
    }

    private static string[] GetConfigurationPaths() {
      return
      [
        // 1. Executable directory (HIGHEST priority - where the built exe and config are)
        Path.Combine(AppContext.BaseDirectory, Cx.CONFIG_FILE_NAME).ResolvePath(),
        
        // 2. Current directory (for dotnet run scenarios)
        Path.Combine(Directory.GetCurrentDirectory(), Cx.CONFIG_FILE_NAME).ResolvePath(),
                
        // 3. User config directory (cross-platform)
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            Cx.AppName,
            Cx.CONFIG_FILE_NAME
        ).ResolvePath(),
                
        // 4. System config directory (cross-platform)
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            Cx.AppName,
            Cx.CONFIG_FILE_NAME
        ).ResolvePath()
      ];
    }

    private Dictionary<string, DaemonsMCP.Core.Models.ProjectModel>? LoadProjectsFromConfig() {
      try {        
        if (_config == null) return null;

        var projects = new Dictionary<string, ProjectModel>();

        foreach (var projectConfig in _config.Projects.Where(p => p.Enabled)) {
          // Validate project configuration
          if (string.IsNullOrEmpty(projectConfig.Name) ||
              string.IsNullOrEmpty(projectConfig.Path)) {
            if (Cx.IsDebug) _logger.LogDebug($"{Cx.Dd0} Skipping invalid project configuration");
            continue;
          }

          // Resolve path (handle relative/absolute)
          var resolvedPath = projectConfig.Path.ResolvePath();

          // Validate path exists
          if (!Directory.Exists(resolvedPath)) {
            if (Cx.IsDebug) _logger.LogDebug($"{Cx.Dd0} Warning: Project path does not exist: {resolvedPath}");
            continue;
          }

          var project = new ProjectModel(
              projectConfig.Name,
              projectConfig.Description,
              resolvedPath
          );

          projects[project.Name] = project;
        }

        if (Cx.IsDebug) _logger.LogDebug($"{Cx.Dd0} Loaded {projects.Count} projects from configuration");
        return projects;
      } catch (Exception ex) {
        if (Cx.IsDebug) _logger.LogDebug($"{Cx.Dd0} Error loading configuration: {ex.Message}");
        return null;
      }
    }

    public void Reload(string? configPath = null ) {
      _config = LoadConfiguration(configPath) ?? new();
      _projects = LoadProjectsFromConfig();
      var projectCount = _projects?.Count ?? 0;
      if (Cx.IsDebug) _logger.LogDebug($"{Cx.Dd0} Reloaded with {projectCount} projects");
    }
  }    

}
