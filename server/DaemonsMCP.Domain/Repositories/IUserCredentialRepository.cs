using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Repositories {
  
  public interface IUserCredentialRepository {
    Task<UserCredential?> GetByIdAsync(int id);
    Task<List<UserCredential>> GetByUserIdAsync(int userId);
    Task<UserCredential?> GetByUserAndProviderAsync(int userId, ProviderType providerType);
    Task<UserCredential> AddAsync(UserCredential credential);
    Task UpdateAsync(UserCredential credential);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int userId, ProviderType providerType, CredentialType credentialType);
  }
}
