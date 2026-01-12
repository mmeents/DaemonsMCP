using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DaemonsMCP.Infrastructure.Repositories {
  public class GitBranchRepository : IGitBranchRepository {
    private readonly DaemonsMcpDbContext _context;

    public GitBranchRepository(DaemonsMcpDbContext context) {
      _context = context;
    }

    public async Task<GitBranch?> GetByIdAsync(int id, CancellationToken cancellationToken = default) {
      return await _context.GitBranches
          .FirstOrDefaultAsync(rb => rb.Id == id, cancellationToken);
    }

    public async Task<GitBranch?> GetCurrentBranchAsync(int gitRepositoryId, CancellationToken cancellationToken = default) {
      return await _context.GitBranches 
          .FirstOrDefaultAsync(rb => rb.GitRepositoryId == gitRepositoryId && rb.IsCurrentBranch, cancellationToken);
    }

    public async Task<GitBranch?> GetByNameAsync(int gitRepositoryId, string branchName, CancellationToken cancellationToken = default) {
      return await _context.GitBranches
          .FirstOrDefaultAsync(rb => rb.GitRepositoryId == gitRepositoryId && rb.BranchName == branchName, cancellationToken);
    }

    public async Task<List<GitBranch>> GetByRepositoryIdAsync(int gitRepositoryId, CancellationToken cancellationToken = default) {
      return await _context.GitBranches
          .Where(rb => rb.GitRepositoryId == gitRepositoryId)
          .OrderBy(rb => rb.IsRemote)
          .ThenBy(rb => rb.BranchName)
          .ToListAsync(cancellationToken);
    }

    public async Task<List<GitBranch>> GetLocalBranchesAsync(int gitRepositoryId, CancellationToken cancellationToken = default) {
      return await _context.GitBranches 
          .Where(rb => rb.GitRepositoryId == gitRepositoryId && !rb.IsRemote)
          .OrderBy(rb => rb.BranchName)
          .ToListAsync(cancellationToken);
    }

    public async Task<List<GitBranch>> GetRemoteBranchesAsync(int gitRepositoryId, CancellationToken cancellationToken = default) {
      return await _context.GitBranches
          .Where(rb => rb.GitRepositoryId == gitRepositoryId && rb.IsRemote)
          .OrderBy(rb => rb.BranchName)
          .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int gitRepositoryId, string fullName, CancellationToken cancellationToken = default) {
      return await _context.GitBranches
          .AnyAsync(rb => rb.GitRepositoryId == gitRepositoryId && rb.FullName == fullName, cancellationToken);
    }

    public async Task<GitBranch> AddAsync(GitBranch branch, CancellationToken cancellationToken = default) {
      await _context.GitBranches.AddAsync(branch, cancellationToken);
      return branch;
    }

    public async Task AddRangeAsync(IEnumerable<GitBranch> branches, CancellationToken cancellationToken = default) {
      await _context.GitBranches.AddRangeAsync(branches, cancellationToken);
    }

    public Task UpdateAsync(GitBranch branch, CancellationToken cancellationToken = default) {
      _context.GitBranches.Update(branch);
      return Task.CompletedTask;
    }

    public Task DeleteAsync(GitBranch branch, CancellationToken cancellationToken = default) {
      _context.GitBranches.Remove(branch);
      return Task.CompletedTask;
    }

    public async Task DeleteByRepositoryIdAsync(int gitRepositoryId, CancellationToken cancellationToken = default) {
      var branches = await _context.GitBranches
          .Where(rb => rb.GitRepositoryId == gitRepositoryId)
          .ToListAsync(cancellationToken);

      _context.GitBranches.RemoveRange(branches);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
      return await _context.SaveChangesAsync(cancellationToken);
    }

    public IQueryable<GitBranch> GetQueryable() {
      return _context.GitBranches.AsQueryable();
    }
  }
}
