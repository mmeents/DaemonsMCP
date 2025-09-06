using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaemonsMCP.Core.Models;

namespace DaemonsMCP.Core.Services {
  public interface IProjectFileService {
    Task<IEnumerable<string>> GetFilesAsync(string projectName, string? path = null, string? filter = null);
    Task<FileContent> GetFileAsync(string projectName, string path);
    Task<OperationResult> CreateFileAsync(string projectName, string path, string content);
    Task<OperationResult> UpdateFileAsync(string projectName, string path, string content);
    Task<OperationResult> DeleteFileAsync(string projectName, string path, bool confirmDeletion = false);
  }
}
