using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Models {
  public class FileContent {
    public string FileName { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public long Size { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string Encoding { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsBinary { get; set; }
  }

}
