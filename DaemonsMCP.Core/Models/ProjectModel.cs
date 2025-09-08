using DaemonsMCP.Core.Config;
using DaemonsMCP.Core.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Models {
  public class ProjectModel {
    public ProjectModel() { }
    public ProjectModel(string Name, string Description, string FullPath) {
      if (string.IsNullOrEmpty(Name)) throw new ArgumentNullException(nameof(Name));
      this.Name = Name ?? throw new ArgumentNullException(nameof(Name));
      this.Description = Description ?? throw new ArgumentNullException(nameof(Description));
      this.Path = FullPath ?? throw new ArgumentNullException(nameof(FullPath));
    }
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

    [JsonPropertyName("path")]
    public string Path { get; set; } = "";

    private string _IndexPath = "";

    [JsonPropertyName("indexPath")]
    public string IndexPath { 
      get {  
        if (!string.IsNullOrEmpty(_IndexPath)) return _IndexPath;
        _IndexPath = System.IO.Path.GetFullPath( System.IO.Path.Combine(this.Path.ResolvePath(), Cx.DaemonsFolderName));
        return _IndexPath;
      }
    }

    private string _BackupPath = "";

    [JsonPropertyName("backupPath")]
    public string BackupPath {
      get {
        if (!string.IsNullOrEmpty(_BackupPath)) return _BackupPath;
        _BackupPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(this.IndexPath, Cx.BackupFolderName));
        return _BackupPath;
      }

    }
  }
}
