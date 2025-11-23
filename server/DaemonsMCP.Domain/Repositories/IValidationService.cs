using DaemonsMCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Repositories {
  public interface IValidationService {

    Task<FileValidationContext> ValidateAndPrepareFolder(int projectId, string relativePath, bool isNewFolder);

    Task<FileValidationContext> ValidateAndPrepareFile(int projectId, string relativePath, bool isNewFile);
    void ValidatePath(string path);
    void ValidateContent(string content);
    Task<FileSystemFilters> GetFileSystemFiltersAsync(CancellationToken cancellationToken = default);

    bool IsWriteAllowed(string filePath, FileSystemFilters filters);
  }

}
