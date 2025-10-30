using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaemonsMCP.Domain.Entities;

namespace DaemonsMCP.Domain.Models {  
  public class FileValidationContext {
    public Project Project { get; set; } = null!;
    public string RelativePath { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
    public FileSystemFilters Filters { get; set; }
  }

}
