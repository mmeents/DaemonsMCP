using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Models {
  public class MethodListing {
    public int MethodId { get; set; }
    public int ClassId { get; set; }
    public string ProjectName { get; set; } = "";
    public string Namespace { get; set; } = "";
    public string ClassName { get; set; } = "";
    public string MethodName { get; set; } = "";
    public string Parameters { get; set; } = "";
    public string ReturnType { get; set; } = "";
    public int LineStart { get; set; }
    public int LineEnd { get; set; }
    public string FileName { get; set; } = "";

  }
}
