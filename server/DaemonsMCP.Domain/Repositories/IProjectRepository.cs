using DaemonsMCP.Domain.Entities;

namespace DaemonsMCP.Domain.Repositories;

public interface IProjectRepository {
  // Queries
  Task<Project?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
  Task<Project?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
  Task<List<Project>> GetAllAsync(CancellationToken cancellationToken = default);
  Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default);

  // Commands
  Task<Project> AddAsync(Project project, CancellationToken cancellationToken = default);
  Task UpdateAsync(Project project, CancellationToken cancellationToken = default);
  Task DeleteAsync(Project project, CancellationToken cancellationToken = default);

  // Unit of Work
  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

}