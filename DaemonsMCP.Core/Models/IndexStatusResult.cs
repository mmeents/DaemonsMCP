using DaemonsMCP.Core.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Models {
  public class IndexStatusResult {       
    public IndexStatusResult() { }
    public bool Enabled {get; set;} = false;
    public List<IndexProjectResult> Projects {get; set;} = new List<IndexProjectResult>();
  }


 

}
