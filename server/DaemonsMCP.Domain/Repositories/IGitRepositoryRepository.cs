using DaemonsMCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Repositories {
  public interface IGitRepositoryRepository {
    Task<GitRepository?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<GitRepository?> GetByIdWithBranchesAsync(int id, CancellationToken cancellationToken = default);
    Task<GitRepository?> GetByPathAsync(int projectId, string localPath, CancellationToken cancellationToken = default);
    Task<List<GitRepository>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
    Task<List<GitRepository>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<GitRepository>> GetDirtyRepositoriesAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int projectId, string localPath, CancellationToken cancellationToken = default);

    // Commands
    Task<GitRepository> AddAsync(GitRepository repository, CancellationToken cancellationToken = default);
    Task UpdateAsync(GitRepository repository, CancellationToken cancellationToken = default);
    Task DeleteAsync(GitRepository repository, CancellationToken cancellationToken = default);
    Task DeleteByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);

    // Unit of Work
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    // Queryable for complex CQRS handlers
    IQueryable<GitRepository> GetQueryable();
  }
}
