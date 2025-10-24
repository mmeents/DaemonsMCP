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

public class IdentifierTypeRepository : IIdentifierTypeRepository {
  private readonly DaemonsMcpDbContext _dbContext;

  public IdentifierTypeRepository(DaemonsMcpDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<List<IdentifierType>> GetAllAsync(CancellationToken cancellationToken = default) {
    return await _dbContext.IdentifierTypes.ToListAsync(cancellationToken);
  }

  public async Task<IdentifierType?> GetByIdAsync(int id, CancellationToken cancellationToken = default) {
    return await _dbContext.IdentifierTypes.FirstOrDefaultAsync(it => it.Id == id, cancellationToken);
  }
}