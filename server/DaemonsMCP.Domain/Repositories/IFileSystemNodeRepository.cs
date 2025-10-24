using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DaemonsMCP.Domain.Entities;

namespace DaemonsMCP.Domain.Repositories;

public interface IFileSystemNodeRepository {
  // Queries
  IQueryable<FileSystemNode> GetQueryable();
  Task<FileSystemNode?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
  Task<FileSystemNode?> GetByPathAsync(int projectId, string relativePath, CancellationToken cancellationToken = default);

  /// <summary>
  /// Gets or creates a FileSystemNode, recursively creating parent directories as needed.
  /// </summary>
  Task<FileSystemNode> GetOrCreateAsync(
      int projectId,
      string relativePath,
      bool isDirectory,
      long? fileSizeBytes = null,
      CancellationToken cancellationToken = default);


  Task<List<FileSystemNode>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
  Task<List<FileSystemNode>> GetChildrenAsync(int projectId, int? parentId, CancellationToken cancellationToken = default);
  Task<List<FileSystemNode>> GetTreeAsync(int projectId, int? parentId, int maxDepth = 10, CancellationToken cancellationToken = default);
  Task<bool> ExistsAsync(int projectId, string relativePath, CancellationToken cancellationToken = default);

  // Commands
  Task<FileSystemNode> AddAsync(FileSystemNode node, CancellationToken cancellationToken = default);
  Task AddRangeAsync(IEnumerable<FileSystemNode> nodes, CancellationToken cancellationToken = default);
  Task UpdateAsync(FileSystemNode node, CancellationToken cancellationToken = default);
  Task DeleteAsync(FileSystemNode node, CancellationToken cancellationToken = default);
  Task DeleteByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);

  // Unit of Work
  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

}
