using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Models {

  public class ClassListing {

    public int ClassId { get; set; }
    public int FileId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string Namespace { get; set; } = string.Empty;
    public string FileNamePath { get; set; } = string.Empty;
    public int LineStart { get; set; }
    public int LineEnd { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public DateTime Modified { get; set; } = DateTime.Now;

  }
}
