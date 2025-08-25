using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Models {
  public class FileChangeItem {
    public string FilePath { get; set; } = "";
    public WatcherChangeTypes ChangeType { get; set; }
    public DateTime Timestamp { get; set; }
  }
}
