using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Models {
  public class IndexEventItem {
    public int Id { get; set; }
    public int ClassId { get; set; }
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public int LineStart { get; set; }
    public int LineEnd { get; set; }
  }

}
