using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Enums;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DaemonsMCP.Infrastructure.Repositories {
  public class UserCredentialRepository: IUserCredentialRepository {
    private readonly DaemonsMcpDbContext _context;
    public UserCredentialRepository(DaemonsMcpDbContext context) {
      _context = context;
    }

    public async Task<UserCredential> AddAsync(UserCredential credential) {
      await _context.UserCredentials.AddAsync(credential);
      await _context.SaveChangesAsync();
      return credential;
    }

    public async Task DeleteAsync(int id) {
      var credential = await _context.UserCredentials.FindAsync(id);
      if (credential != null) {
        _context.UserCredentials.Remove(credential);
        await _context.SaveChangesAsync();
      }
    }

    public async Task<bool> ExistsAsync(int userId, ProviderType providerType, CredentialType credentialType) {
      return await _context.UserCredentials.AnyAsync(uc =>
        uc.UserId == userId &&
        uc.ProviderType == providerType &&
        uc.CredentialType == credentialType);
    }

    public async Task<UserCredential?> GetByIdAsync(int id) {
      return await _context.UserCredentials.FindAsync(id);
    }

    public async Task<UserCredential?> GetByUserAndProviderAsync(int userId, ProviderType providerType) {
      return await _context.UserCredentials
        .FirstOrDefaultAsync(uc =>
          uc.UserId == userId &&
          uc.ProviderType == providerType);
    }

    public async Task<List<UserCredential>> GetByUserIdAsync(int userId) {
      return await _context.UserCredentials
        .Where(uc => uc.UserId == userId)
        .ToListAsync();
    }

    public async Task UpdateAsync(UserCredential credential) {
      _context.UserCredentials.Update(credential);
      await _context.SaveChangesAsync();
    }
  }
}
