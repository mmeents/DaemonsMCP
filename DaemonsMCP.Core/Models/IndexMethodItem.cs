using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Models {
  public class IndexMethodItem {
    public IndexMethodItem() { }
    public int Id { get; set; } = 0;
    public int ClassId { get; set; } = 0; // Reference to IndexClassItem
    public string Name { get; set; } = "";
    public string ReturnType { get; set; } = "";
    public string Parameters { get; set; } = ""; // JSON string of parameters
    public int LineStart { get; set; } = 0;
    public int LineEnd { get; set; } = 0;

  }

}
