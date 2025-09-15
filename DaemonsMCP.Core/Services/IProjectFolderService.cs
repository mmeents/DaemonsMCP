using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaemonsMCP.Core.Models;

namespace DaemonsMCP.Core.Services {
  public interface IProjectFolderService {
   
    Task<IEnumerable<string>> GetFoldersAsync(string projectName, string? path = null, string? filter = null);
    Task<OperationResult> CreateFolderAsync(string projectName, string path, bool createParents = true);
    Task<OperationResult> DeleteFolderAsync(string projectName, string path, bool recursive = false, bool confirmDeletion = false);
  
  }
}
