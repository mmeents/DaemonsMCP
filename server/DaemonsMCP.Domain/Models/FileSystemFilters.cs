using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Models {
  public record FileSystemFilters(
      HashSet<string> BlockedFolders,
      HashSet<string> BlockedExtensions,
      HashSet<string> AllowedExtensions,
      HashSet<string> BlockedFiles
  );
}
