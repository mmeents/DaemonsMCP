using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Application.FileSystem.Queries.GetFileContents {
  public class GetFileContentsResult {
    public int FileSystemNodeId { get; set; }
    public int ProjectId { get; set; }
    public long? SizeInBytes  { get; set; }
    public string RelativePath { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    
  }
}
