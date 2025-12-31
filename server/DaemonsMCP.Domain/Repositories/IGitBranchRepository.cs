using DaemonsMCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Repositories {
  public interface IGitBranchRepository {
    Task<GitBranch?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<GitBranch?> GetCurrentBranchAsync(int gitRepositoryId, CancellationToken cancellationToken = default);
    Task<GitBranch?> GetByNameAsync(int gitRepositoryId, string branchName, CancellationToken cancellationToken = default);
    Task<List<GitBranch>> GetByRepositoryIdAsync(int gitRepositoryId, CancellationToken cancellationToken = default);
    Task<List<GitBranch>> GetLocalBranchesAsync(int gitRepositoryId, CancellationToken cancellationToken = default);
    Task<List<GitBranch>> GetRemoteBranchesAsync(int gitRepositoryId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int gitRepositoryId, string fullName, CancellationToken cancellationToken = default);

    // Commands
    Task<GitBranch> AddAsync(GitBranch branch, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<GitBranch> branches, CancellationToken cancellationToken = default);
    Task UpdateAsync(GitBranch branch, CancellationToken cancellationToken = default);
    Task DeleteAsync(GitBranch branch, CancellationToken cancellationToken = default);
    Task DeleteByRepositoryIdAsync(int gitRepositoryId, CancellationToken cancellationToken = default);

    // Unit of Work
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    // Queryable for complex CQRS handlers
    IQueryable<GitBranch> GetQueryable();
  }
}
