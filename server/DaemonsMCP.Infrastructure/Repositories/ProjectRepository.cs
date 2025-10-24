using Microsoft.EntityFrameworkCore;
using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Infrastructure.Persistence;

namespace DaemonsMCP.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository {
  private readonly DaemonsMcpDbContext _context;

  public ProjectRepository(DaemonsMcpDbContext context) {
    _context = context;
  }

  public async Task<Project?> GetByIdAsync(int id, CancellationToken cancellationToken = default) {
    return await _context.Projects
        .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
  }

  public async Task<Project?> GetByNameAsync(string name, CancellationToken cancellationToken = default) {
    return await _context.Projects
        .FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
  }

  public async Task<List<Project>> GetAllAsync(CancellationToken cancellationToken = default) {
    return await _context.Projects
        .OrderBy(p => p.Name)
        .ToListAsync(cancellationToken);
  }

  public async Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default) {
    return await _context.Projects
        .AnyAsync(p => p.Name == name, cancellationToken);
  }

  public async Task<Project> AddAsync(Project project, CancellationToken cancellationToken = default) {
    await _context.Projects.AddAsync(project, cancellationToken);
    return project;
  }

  public Task UpdateAsync(Project project, CancellationToken cancellationToken = default) {
    _context.Projects.Update(project);
    return Task.CompletedTask;
  }

  public Task DeleteAsync(Project project, CancellationToken cancellationToken = default) {
    _context.Projects.Remove(project);
    return Task.CompletedTask;
  }

  public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
    return await _context.SaveChangesAsync(cancellationToken);
  }
}