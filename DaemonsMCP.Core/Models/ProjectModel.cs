using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DaemonsMCP.Core.Config;
using DaemonsMCP.Core.Extensions;

namespace DaemonsMCP.Core.Models {
  public class ProjectModel {
    public ProjectModel() { }
    public ProjectModel(string Name, string Description, string FullPath) {
      if (string.IsNullOrEmpty(Name)) throw new ArgumentNullException(nameof(Name));
      if (string.IsNullOrEmpty(FullPath)) throw new ArgumentNullException(nameof(FullPath));
      this.Name = Name ?? throw new ArgumentNullException(nameof(Name));
      this.Description = Description ?? throw new ArgumentNullException(nameof(Description));
      this.Path = FullPath ?? throw new ArgumentNullException(nameof(FullPath));
    }
    public int Id { get; set; } = 0;
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Path { get; set; } = "";

    private string _IndexPath = "";
    public string IndexPath { 
      get {  
        if (!string.IsNullOrEmpty(_IndexPath)) return _IndexPath;
        _IndexPath = Sx.CommonAppPath;
        return _IndexPath;
      }
    }
    
  }
}
