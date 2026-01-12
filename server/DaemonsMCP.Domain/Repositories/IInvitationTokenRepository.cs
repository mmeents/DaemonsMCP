using DaemonsMCP.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Repositories {
  public interface IInvitationTokenRepository {

    // Queries
    IQueryable<InvitationToken> GetQueryable();
    Task<InvitationToken?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<InvitationToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<string> FindUnusedNewToken(CancellationToken cancellationToken = default);

    
    // Commands
    Task<InvitationToken?> AddAsync(InvitationToken token, CancellationToken cancellationToken = default);
    Task UpdateAsync(InvitationToken token, CancellationToken cancellationToken = default);
    Task DeleteAsync(InvitationToken token, CancellationToken cancellationToken = default);

    // Unit of Work
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

  }
}
