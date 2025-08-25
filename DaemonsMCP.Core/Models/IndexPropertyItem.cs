using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Models {
  public class IndexPropertyItem {
    public int Id { get; set; }
    public int ClassId { get; set; }
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public int LineStart { get; set; }
    public int LineEnd { get; set; }
    public bool HasGetter { get; set; }
    public bool HasSetter { get; set; }
  }

}
