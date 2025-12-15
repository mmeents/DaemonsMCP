using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Extensions;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading;

namespace DaemonsMCP.Infrastructure.Repositories;

public class AccessTokenRepository : IAccessTokenRepository {
  private readonly DaemonsMcpDbContext _context;
  private readonly int _tokenByteLength;

  public AccessTokenRepository(DaemonsMcpDbContext context, int tokenByteLength = 8) {
    _context = context;
    _tokenByteLength = tokenByteLength;
  }

  public async Task<AccessToken?> GetByIdAsync(int id, CancellationToken cancellationToken = default) {
    return await _context.AccessTokens
        .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
  }

  public async Task<AccessToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default) {
    var now = DateTime.UtcNow;
    var accessToken =await _context.AccessTokens
        .FirstOrDefaultAsync(a => a.Token == token, cancellationToken);
    if (accessToken?.ParentId != null) {
      await _context.AccessTokens
        .Include(a => a.Parent)
        .FirstOrDefaultAsync(a => a.Id == accessToken.ParentId, cancellationToken);
    }
    if (accessToken?.Parent != null && accessToken.Parent.IsRevokedChain) { 
      return null;
    }
    return accessToken;
  }

  public async Task<AccessToken?> GetByIdWithChildrenAsync(int id, int maxDepth = 1, CancellationToken cancellationToken = default) {
    var query = _context.AccessTokens.AsQueryable();

    // Recursively include children based on maxDepth
    for (int i = 0; i < maxDepth; i++) {
      query = query.Include(token => token.Children);
    }

    return await query.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
  }

  public async Task<List<AccessToken>> GetAllAsync(CancellationToken cancellationToken = default) {
    return await _context.AccessTokens
        .OrderByDescending(a => a.Created)
        .ToListAsync(cancellationToken);
  }

  public async Task<List<AccessToken>> GetByParentIdAsync(int? parentId, CancellationToken cancellationToken = default) {
    return await _context.AccessTokens
        .Where(a => a.ParentId == parentId)
        .OrderByDescending(a => a.Created)
        .ToListAsync(cancellationToken);
  }

  public async Task<List<AccessToken>> GetValidTokensAsync(CancellationToken cancellationToken = default) {
    var now = DateTime.UtcNow;
    return await _context.AccessTokens
        .Where(a => a.Expires > now && a.Used == null)
        .OrderByDescending(a => a.Created)
        .ToListAsync(cancellationToken);
  }

  public async Task<List<AccessToken>> GetExpiredTokensAsync(CancellationToken cancellationToken = default) {
    var now = DateTime.UtcNow;
    return await _context.AccessTokens
        .Where(a => a.Expires <= now)
        .OrderByDescending(a => a.Created)
        .ToListAsync(cancellationToken);
  }

  public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default) {
    return await _context.AccessTokens
        .AnyAsync(a => a.Id == id, cancellationToken);
  }

  public async Task<bool> TokenExistsAsync(string token, CancellationToken cancellationToken = default) {
    var now = DateTime.UtcNow;
    var accessToken = await _context.AccessTokens
        .FirstOrDefaultAsync(a => a.Token == token && a.Expires >= now && a.Used == null, cancellationToken);
    if ((accessToken?.ParentId ?? 0) != 0) {
      accessToken = await _context.AccessTokens
        .Include(a => a.Parent)
        .FirstOrDefaultAsync(a => a.Id == accessToken.ParentId, cancellationToken);
    }
    if (accessToken?.Parent != null && accessToken.Parent.IsRevokedChain) {
      return false;
    }
    return accessToken != null;
  }

  public async Task<AccessToken> AddAsync(AccessToken token, CancellationToken cancellationToken = default) {
    await _context.AccessTokens.AddAsync(token, cancellationToken);
    return token;
  }

  public Task UpdateAsync(AccessToken token, CancellationToken cancellationToken = default) {
    _context.AccessTokens.Update(token);
    return Task.CompletedTask;
  }

  public Task DeleteAsync(AccessToken token, CancellationToken cancellationToken = default) {
    _context.AccessTokens.Remove(token);
    return Task.CompletedTask;
  }

  public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
    return await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task<string> FindUnusedNewToken(CancellationToken cancellationToken = default) {
    int maxAttempts = 1000;
    while (maxAttempts > 0) {
      var token = TokenExt.GenerateToken(_tokenByteLength);
      var accessToken = await _context.AccessTokens.FirstOrDefaultAsync(a => a.Token == token, cancellationToken);
      if (accessToken == null) {
        return token;
      }
      maxAttempts--;
    }
    throw new InvalidOperationException("Failed to generate unique token after maximum attempts");
  }

  public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default) {
    return await _context.Database.BeginTransactionAsync(cancellationToken);
  }

  public IQueryable<AccessToken> GetQueryable() {
    return _context.AccessTokens.AsQueryable();
  }
}
