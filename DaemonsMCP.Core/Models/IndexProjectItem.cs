using DaemonsMCP.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Models {
  public class IndexProjectItem {
    public string Name { get; set; } = "";    
    public string Path { get; set; } = "";
    public ProjectIndexModel? ProjectIndex { get; set; } = null;
  }
}
