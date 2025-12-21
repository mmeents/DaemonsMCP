using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DaemonsMCP.Domain.Entities;

namespace DaemonsMCP.Domain.Repositories;

public interface IUserRepository {
  // Queries
  IQueryable<User> GetQueryable();
  Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
  Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
  Task<User?> GetByGoogleIdAsync(string googleId, CancellationToken cancellationToken = default);
  Task<User?> GetByGitHubIdAsync(string gitHubId, CancellationToken cancellationToken = default);
  Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
  Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default);

  // Commands
  Task<User> AddAsync(User user, CancellationToken cancellationToken = default);
  Task UpdateAsync(User user, CancellationToken cancellationToken = default);
  Task DeleteAsync(User user, CancellationToken cancellationToken = default);

  // Unit of Work
  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
