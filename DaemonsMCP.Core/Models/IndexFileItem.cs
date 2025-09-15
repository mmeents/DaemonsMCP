using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Models {
  public class IndexFileItem {   
      public IndexFileItem() { }
      public int Id { get; set; } = 0;
      public string FilePathName { get; set; } = "";
      public long Size { get; set; } = 0;
      public DateTime Modified {get; set;} = DateTime.Now;
  }
}
