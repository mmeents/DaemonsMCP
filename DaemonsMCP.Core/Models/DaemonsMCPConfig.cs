using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Models {
   
  public class DaemonsMcpConfiguration {

    [JsonPropertyName("daemon")]   
    public VersionSettings Version { get; set; } = new();

    [JsonPropertyName("projects")]
    public List<ProjectSettings> Projects { get; set; } = new();

    [JsonPropertyName("security")]
    public SecuritySettings Security { get; set; } = new();
  }
}
