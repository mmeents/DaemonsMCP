using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Application.FileSystem.Services;

public interface IFileSystemSyncService {
  /// <summary>
  /// Syncs filesystem to database, preserving existing IDs where possible
  /// </summary>
  Task<SyncResult> SyncProjectAsync(DaemonsMCP.Domain.Entities.Project project, CancellationToken cancellationToken = default);
}

public record SyncResult(
    int FilesAdded,
    int FilesUpdated,
    int FilesDeleted,
    int DirectoriesAdded,
    int DirectoriesDeleted,
    TimeSpan Duration
);
