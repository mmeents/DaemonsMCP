using DaemonsMCP.Infrastructure.Persistence;
using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Threading;


namespace DaemonsMCP.Infrastructure.Repositories {
  internal class GitRepositoryRepository : IGitRepositoryRepository {
    private readonly DaemonsMcpDbContext _context;

    public GitRepositoryRepository(DaemonsMcpDbContext context) {
      _context = context;
    }

    public async Task<GitRepository?> GetByIdAsync(int id, CancellationToken cancellationToken = default) {
      return await _context.GitRepositories
        .Include(gr => gr.Project)
        .Include(gr => gr.Branches.OrderBy(b => b.IsRemote).ThenBy(b => b.BranchName))
        .FirstOrDefaultAsync(gr => gr.Id == id, cancellationToken);
          
    }

    public async Task<GitRepository?> GetByIdWithBranchesAsync(int id, CancellationToken cancellationToken = default) {
      return await _context.GitRepositories
          .Include(gr => gr.Branches.OrderBy(b => b.IsRemote).ThenBy(b => b.BranchName))
          .FirstOrDefaultAsync(gr => gr.Id == id, cancellationToken);
    }

    public async Task<GitRepository?> GetByPathAsync(int projectId, string localPath, CancellationToken cancellationToken = default) {
      return await _context.GitRepositories
          .FirstOrDefaultAsync(gr => gr.ProjectId == projectId && gr.LocalPath == localPath, cancellationToken);
    }

    public async Task<List<GitRepository>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default) {
      return await _context.GitRepositories
          .Where(gr => gr.ProjectId == projectId)
          .OrderBy(pr => pr.LocalPath)
          .ToListAsync(cancellationToken);
    }

    public async Task<List<GitRepository>> GetAllAsync(CancellationToken cancellationToken = default) {
      return await _context.GitRepositories
          .Include(gr => gr.Project)
          .OrderBy(gr => gr.Project.Name)
          .ThenBy(gr => gr.LocalPath)
          .ToListAsync(cancellationToken);
    }

    public async Task<List<GitRepository>> GetDirtyRepositoriesAsync(CancellationToken cancellationToken = default) {
      return await _context.GitRepositories
          .Where(gr => gr.IsDirty)
          .Include(gr => gr.Project)
          .OrderBy(gr => gr.Project.Name)
          .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int projectId, string localPath, CancellationToken cancellationToken = default) {
      return await _context.GitRepositories
          .AnyAsync(gr => gr.ProjectId == projectId && gr.LocalPath == localPath, cancellationToken);
    }

    public async Task<GitRepository> AddAsync(GitRepository repository, CancellationToken cancellationToken = default) {
      await _context.GitRepositories.AddAsync(repository, cancellationToken);
      return repository;
    }

    public Task UpdateAsync(GitRepository repository, CancellationToken cancellationToken = default) {
      _context.GitRepositories.Update(repository);
      return Task.CompletedTask;
    }

    public Task DeleteAsync(GitRepository repository, CancellationToken cancellationToken = default) {
      _context.GitRepositories.Remove(repository);
      return Task.CompletedTask;
    }

    public async Task DeleteByProjectIdAsync(int projectId, CancellationToken cancellationToken = default) {
      var repositories = await _context.GitRepositories
          .Where(gr => gr.ProjectId == projectId)
          .ToListAsync(cancellationToken);

      _context.GitRepositories.RemoveRange(repositories);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
      return await _context.SaveChangesAsync(cancellationToken);
    }

    public IQueryable<GitRepository> GetQueryable() {
      return _context.GitRepositories.AsQueryable();
    }
  }
}
