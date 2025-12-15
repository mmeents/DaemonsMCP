using DaemonsMCP.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace DaemonsMCP.Domain.Repositories;

public interface IAccessTokenRepository {
  // Queries
  IQueryable<AccessToken> GetQueryable();
  Task<AccessToken?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
  Task<AccessToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
  Task<AccessToken?> GetByIdWithChildrenAsync(int id, int maxDepth = 1, CancellationToken cancellationToken = default);
  Task<List<AccessToken>> GetAllAsync(CancellationToken cancellationToken = default);
  Task<List<AccessToken>> GetByParentIdAsync(int? parentId, CancellationToken cancellationToken = default);
  Task<List<AccessToken>> GetValidTokensAsync(CancellationToken cancellationToken = default); // Not expired, not used
  Task<List<AccessToken>> GetExpiredTokensAsync(CancellationToken cancellationToken = default);
  Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
  Task<bool> TokenExistsAsync(string token, CancellationToken cancellationToken = default);
  Task<string> FindUnusedNewToken(CancellationToken cancellationToken = default);

  // Commands
  Task<AccessToken> AddAsync(AccessToken token, CancellationToken cancellationToken = default);
  Task UpdateAsync(AccessToken token, CancellationToken cancellationToken = default);
  Task DeleteAsync(AccessToken token, CancellationToken cancellationToken = default);

  // Unit of Work
  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
  Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

}
