using DaemonsMCP.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Models {
  public class VersionSettings {

    [JsonPropertyName("name")]
    public string Name { get; set; } = Cx.AppName;

    [JsonPropertyName("version")]
    public string Version { get; set; } = Cx.AppVersion;

    [JsonPropertyName("logLevel")]
    public string LogLevel { get; set; } = "Information";
  }
}
