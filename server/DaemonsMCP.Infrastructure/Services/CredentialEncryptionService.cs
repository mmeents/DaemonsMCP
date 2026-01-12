using Microsoft.AspNetCore.DataProtection;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Domain.Constants;

namespace DaemonsMCP.Infrastructure.Services {


  public class CredentialEncryptionService : ICredentialEncryptionService {
    private readonly IDataProtector _protector;

    public CredentialEncryptionService(IDataProtectionProvider provider) {
      // Creates a purpose-specific protector
      _protector = provider.CreateProtector(Cx.CredentialProtectorName);
    }

    public string Encrypt(string plaintext) {
      return _protector.Protect(plaintext);
    }

    public string Decrypt(string ciphertext) {
      return _protector.Unprotect(ciphertext);
    }
  }
}
