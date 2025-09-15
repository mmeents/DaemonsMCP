using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Models {

  public class IndexProjectResult {
    public IndexProjectResult() {
    }
    public string ProjectName { get; set; } = "";    
    public int FileCount { get; set; } = 0;
    public int ClassCount { get; set; } = 0;
    public int MethodCount { get; set; } = 0;
    public int IndexQueuedCount { get; set; } = 0;
  }

}
