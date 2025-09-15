using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Models {
  public class ValidationContext {
    public ProjectModel Project { get; set; } = null!;
    public string RelativePath { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
  }

}
