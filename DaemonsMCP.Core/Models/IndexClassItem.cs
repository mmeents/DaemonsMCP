using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Models {
  public class IndexClassItem {    
      public IndexClassItem() { }
      public int Id { get; set; } = 0;
      public int FileItemId { get; set; } = 0; // Reference to IndexFileItem
      public string Name { get; set; } = "";
      public string Namespace { get; set; } = "";
      public string FileName { get; set; } = "";
      public int LineStart { get; set; } = 0;
      public int LineEnd { get; set; } = 0;
    }
  }
