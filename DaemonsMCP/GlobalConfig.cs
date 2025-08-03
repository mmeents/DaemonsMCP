using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP {
  public static class GlobalConfig {
    private static DaemonsMcpConfiguration? _config;
    private static IReadOnlyDictionary<string, Project>? _projects;

    public static void Initialize() {
      // Load configuration
      _config = ConfigurationLoader.LoadConfiguration();

      // Load projects (config first, fallback to hardcoded)
      _projects = ConfigurationLoader.LoadProjectsFromConfig() ?? Nx.Projects;

      Console.Error.WriteLine($"[DaemonsMCP] Initialized with {_projects.Count} projects");
    }

    // Properties for easy access
    public static SecuritySettings Security => _config?.Security ?? new SecuritySettings();
    public static DaemonSettings Daemon => _config?.Daemon ?? new DaemonSettings();
    public static IReadOnlyDictionary<string, Project> Projects => _projects ?? new Dictionary<string, Project>();

    public static bool IsConfigured => _config != null;
  }
}
