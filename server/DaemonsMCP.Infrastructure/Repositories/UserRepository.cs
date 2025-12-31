using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DaemonsMCP.Infrastructure.Repositories;

public class UserRepository : IUserRepository {
  private readonly DaemonsMcpDbContext _context;

  public UserRepository(DaemonsMcpDbContext context) {
    _context = context;
  }

  public IQueryable<User> GetQueryable() {
    return _context.Users.AsQueryable();
  }

  public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default) {
    return await _context.Users
        .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
  }

  public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) {
    return await _context.Users
        .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);
  }

  public async Task<User?> GetByGoogleIdAsync(string googleId, CancellationToken cancellationToken = default) {
    return await _context.Users
        .FirstOrDefaultAsync(u => u.GoogleId == googleId, cancellationToken);
  }

  public async Task<User?> GetByGitHubIdAsync(string gitHubId, CancellationToken cancellationToken = default) {
    return await _context.Users
        .FirstOrDefaultAsync(u => u.GitHubId == gitHubId, cancellationToken);
  }

  public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default) {
    return await _context.Users
        .AnyAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);
  }

  public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default) {
    return await _context.Users
        .OrderBy(u => u.DisplayName)
        .ToListAsync(cancellationToken);
  }

  public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default) {
    await _context.Users.AddAsync(user, cancellationToken);
    return user;
  }

  public Task UpdateAsync(User user, CancellationToken cancellationToken = default) {
    _context.Users.Update(user);
    return Task.CompletedTask;
  }

  public Task DeleteAsync(User user, CancellationToken cancellationToken = default) {
    _context.Users.Remove(user);
    return Task.CompletedTask;
  }

  public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
    return await _context.SaveChangesAsync(cancellationToken);
  }
}
