using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Application.FileSystem.Queries.SearchFileSystem { 
  public class SearchFileSystemResult {
    public List<FileSystemNodeDto> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNo { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
  }

  public class FileSystemNodeDto {
    public int Id { get; set; }
    public int? ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RelativePath { get; set; } = string.Empty;
    public bool IsDirectory { get; set; }
    public long? SizeInBytes { get; set; }
  }

}
