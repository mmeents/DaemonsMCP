using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Models {
  public class Nodes : BaseItemModel {
    public Nodes() { }

    public string TypeName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;    
    public List<Nodes> Subnodes { get; set; } = new List<Nodes>();    

  }
}
