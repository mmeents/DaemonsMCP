using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Repositories {
  public interface ICredentialEncryptionService {
    string Encrypt(string plaintext);
    string Decrypt(string ciphertext);
  }
}
