using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DaemonsMCP.Infrastructure.Repositories;

public class IdentifierRepository : IIdentifierRepository {
  private readonly DaemonsMcpDbContext _dbContext;

  public IdentifierRepository(DaemonsMcpDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<Identifier?> GetByNameAsync(string name, CancellationToken cancellationToken = default) {
    return await _dbContext.Identifiers
        .FirstOrDefaultAsync(i => i.Name == name, cancellationToken);
  }

  public async Task<Identifier> GetOrCreateAsync(string name, CancellationToken cancellationToken = default) {
    var existing = await GetByNameAsync(name, cancellationToken);
    if (existing != null) {
      return existing;
    }

    var newIdentifier = Identifier.Create(name);
    _dbContext.Identifiers.Add(newIdentifier);
    await _dbContext.SaveChangesAsync(cancellationToken);
    return newIdentifier;
  }

  public async Task<List<Identifier>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default) {
    return await _dbContext.Identifiers
        .Where(i => ids.Contains(i.Id))
        .ToListAsync(cancellationToken);
  }

  public async Task SaveChangesAsync(CancellationToken cancellationToken = default) {
    await _dbContext.SaveChangesAsync(cancellationToken);
  }
}
