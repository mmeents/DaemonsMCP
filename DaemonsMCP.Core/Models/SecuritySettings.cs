using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Models {
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
