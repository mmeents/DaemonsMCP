using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DaemonsMCP {
  public static class ConfigurationLoader {
    private const string CONFIG_FILE_NAME = "daemonsmcp.json";

    public static IReadOnlyDictionary<string, Project>? LoadProjectsFromConfig() {
      try {
        var config = LoadConfiguration();
        if (config == null) return null;

        var projects = new Dictionary<string, Project>();

        foreach (var projectConfig in config.Projects.Where(p => p.Enabled)) {
          // Validate project configuration
          if (string.IsNullOrWhiteSpace(projectConfig.Name) ||
              string.IsNullOrWhiteSpace(projectConfig.Path)) {
            Console.Error.WriteLine($"[DaemonsMCP][Config] Skipping invalid project configuration");
            continue;
          }

          // Resolve path (handle relative/absolute)
          var resolvedPath = ResolvePath(projectConfig.Path);

          // Validate path exists
          if (!Directory.Exists(resolvedPath)) {
            Console.Error.WriteLine($"[DaemonsMCP][Config] Warning: Project path does not exist: {resolvedPath}");
            continue;
          }

          var project = new Project(
              projectConfig.Name,
              projectConfig.Description,
              resolvedPath
          );

          projects[project.Name] = project;
        }

        Console.Error.WriteLine($"[DaemonsMCP][Config] Loaded {projects.Count} projects from configuration");
        return projects;
      } catch (Exception ex) {
        Console.Error.WriteLine($"[DaemonsMCP][Config] Error loading configuration: {ex.Message}");
        return null;
      }
    }

    public static DaemonsMcpConfiguration? LoadConfiguration() {
      var configPaths = GetConfigurationPaths();

      foreach (var configPath in configPaths) {
        if (!File.Exists(configPath)) continue;

        try {
          var jsonContent = File.ReadAllText(configPath);
          var config = JsonSerializer.Deserialize<DaemonsMcpConfiguration>(jsonContent, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
          });

          if (config != null) {
            Console.Error.WriteLine($"[DaemonsMCP][Config] Loaded configuration from: {configPath}");
            return config;
          }
        } catch (JsonException ex) {
          Console.Error.WriteLine($"[DaemonsMCP][Config] Invalid JSON in {configPath}: {ex.Message}");
        } catch (Exception ex) {
          Console.Error.WriteLine($"[DaemonsMCP][Config] Error reading {configPath}: {ex.Message}");
        }
      }

      return null;
    }

    private static string[] GetConfigurationPaths() {
      return new[]
      {
        // 1. Executable directory (HIGHEST priority - where the built exe and config are)
        Path.Combine(AppContext.BaseDirectory, CONFIG_FILE_NAME),
        
        // 2. Current directory (for dotnet run scenarios)
        Path.Combine(Directory.GetCurrentDirectory(), CONFIG_FILE_NAME),
                
        // 3. User config directory (cross-platform)
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "DaemonsMCP",
            CONFIG_FILE_NAME
        ),
                
        // 4. System config directory (cross-platform)
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "DaemonsMCP",
            CONFIG_FILE_NAME
        )



      };
    }

    private static string ResolvePath(string path) {
      // Handle relative paths
      if (!Path.IsPathRooted(path)) {
        // Relative to config file directory or current directory
        return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), path));
      }

      // Handle cross-platform path separators
      return Path.GetFullPath(path);
    }

    public static void CreateSampleConfiguration(string path) {
      var sampleConfig = new DaemonsMcpConfiguration {
        Projects = new List<ProjectConfiguration>
          {
                    new ProjectConfiguration
                    {
                        Name = "SampleProject",
                        Description = "A sample project configuration",
                        Path = "./sample-project",
                        Enabled = true
                    }
                }
      };

      var jsonOptions = new JsonSerializerOptions {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
      };

      var jsonContent = JsonSerializer.Serialize(sampleConfig, jsonOptions);
      File.WriteAllText(path, jsonContent);

      Console.WriteLine($"Sample configuration created at: {path}");
    }
  }
}
