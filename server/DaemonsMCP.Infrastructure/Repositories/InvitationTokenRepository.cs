using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Extensions;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DaemonsMCP.Infrastructure.Repositories {
  public class InvitationTokenRepository : IInvitationTokenRepository {
    private readonly DaemonsMcpDbContext _context;
    private readonly int _tokenBytesLength;

    public InvitationTokenRepository(DaemonsMcpDbContext context, int tokenBytesLength = 8) {
      _context = context;
      _tokenBytesLength = tokenBytesLength;
    }

    public async Task<InvitationToken?> AddAsync(InvitationToken token, CancellationToken cancellationToken = default) {
      string thisToken = token.Token;
      await _context.InvitationTokens.AddAsync(token, cancellationToken);
      await _context.SaveChangesAsync(cancellationToken);
      var addedToken = await _context.InvitationTokens
          .FirstOrDefaultAsync(t => t.Token == thisToken, cancellationToken);
      return addedToken;
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default) {
      return await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task DeleteAsync(InvitationToken token, CancellationToken cancellationToken = default) {
      _context.InvitationTokens.Remove(token);
      await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<string> FindUnusedNewToken(CancellationToken cancellationToken = default) {
      int maxAttempts = 1000;
      while (maxAttempts > 0) {
        var token = TokenExt.GenerateToken(_tokenBytesLength);
        var accessToken = await _context.InvitationTokens.FirstOrDefaultAsync(a => a.Token == token, cancellationToken);
        if (accessToken == null) {
          return token;
        }
        maxAttempts--;
      }
      throw new InvalidOperationException("Failed to generate unique token after maximum attempts");
    }

    public async Task<InvitationToken?> GetByIdAsync(int id, CancellationToken cancellationToken = default) {
      return await _context.InvitationTokens
          .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<InvitationToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default) {
      return await _context.InvitationTokens
          .FirstOrDefaultAsync(t => t.Token == token, cancellationToken);
    }

    public IQueryable<InvitationToken> GetQueryable() {
      return _context.InvitationTokens.AsQueryable();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
      return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(InvitationToken token, CancellationToken cancellationToken = default) {
      _context.InvitationTokens.Update(token);
      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
