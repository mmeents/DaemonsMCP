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
    
    [JsonPropertyName("writeProtectedPaths")]
    public List<string> WriteProtectedPaths { get; set; } = new()
    {
            "etc", "var", "usr/local", "bin", "sbin"
    };

    [JsonPropertyName("BlockedFileNames")]
    public List<string> BlockedFileNames { get; set; } = new()
    {
            "appsettings.json","appsettings.Development.json", "appsettings.Production.json","web.config", 
            "app.config","secrets.json", "user-secrets.json", ".gitignore","database.db",
            "id_rsa", "id_ed25519", "id_rsa.pub", "id_ed25519.pub"
        };

    [JsonPropertyName("allowWrite")]
    public bool AllowWrite { get; set; } = false;


    [JsonPropertyName("maxFileSize")]
    public string MaxFileSize { get; set; } = "10MB";



    [JsonPropertyName("maxFileWriteSize")]
    public string MaxFileWriteSize { get; set; } = "5MB";

  }

}
