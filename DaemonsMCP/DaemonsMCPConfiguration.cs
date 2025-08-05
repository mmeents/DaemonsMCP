using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DaemonsMCP {
  public class DaemonsMcpConfiguration {
    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.0";

    [JsonPropertyName("daemon")]
    public DaemonSettings Daemon { get; set; } = new();

    [JsonPropertyName("projects")]
    public List<ProjectConfiguration> Projects { get; set; } = new();

    [JsonPropertyName("security")]
    public SecuritySettings Security { get; set; } = new();
  }

  public class DaemonSettings {
    [JsonPropertyName("name")]
    public string Name { get; set; } = "DaemonsMCP";

    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.0.0";

    [JsonPropertyName("logLevel")]
    public string LogLevel { get; set; } = "Information";
  }

  public class ProjectConfiguration {
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

    [JsonPropertyName("path")]
    public string Path { get; set; } = "";

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; } = true;
  }

  public class SecuritySettings {
    [JsonPropertyName("allowedExtensions")]
    public List<string> AllowedExtensions { get; set; } = new()
    {
            ".cs", ".js", ".py", ".md", ".txt", ".json", ".xml", ".yml", ".yaml"
        };

    [JsonPropertyName("blockedExtensions")]
    public List<string> BlockedExtensions { get; set; } = new()
    {
            ".exe", ".dll", ".bin", ".so", ".dylib"
        };

    [JsonPropertyName("maxFileSize")]
    public string MaxFileSize { get; set; } = "10MB";

    [JsonPropertyName("maxDirectoryDepth")]
    public int MaxDirectoryDepth { get; set; } = 10;

    [JsonPropertyName("allowWrite")]
    public bool AllowWrite { get; set; } = false;

    [JsonPropertyName("writeProtectedPaths")]
    public List<string> WriteProtectedPaths { get; set; } = new()
    {
            "etc", "var", "usr/local", "bin", "sbin"
    };

    [JsonPropertyName("maxFileWriteSize")]
    public string MaxFileWriteSize { get; set; } = "5MB";

  }
}
