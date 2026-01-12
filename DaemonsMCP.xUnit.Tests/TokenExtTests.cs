using DaemonsMCP.Domain.Constants;
using DaemonsMCP.Domain.Extensions;
using DaemonsMCP.Domain.Repositories;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using DaemonsMCP.Infrastructure.Services;

namespace DaemonsMCP.xUnit.Tests {
  public class TokenExtTests {
    [Fact]
    public void Test1() {

      string token = TokenExt.GenerateToken();
      int len = token.Length;
      HashSet<string> tokens = new HashSet<string>();

      for (int i = 0; i < 400; i++) {
        string newToken = TokenExt.GenerateToken();
        if (!tokens.Add(newToken)) Console.WriteLine("dup: "+newToken+" ");
      }

      Assert.NotNull(token);

    }

    [Fact]
    public void TestDataProtection() {
      var services = new ServiceCollection();
      services.AddDataProtection()
        .SetApplicationName(Cx.AppName)
        .PersistKeysToFileSystem(new DirectoryInfo(CommonPath.KeysAppPath)) // or config-driven path
        .SetDefaultKeyLifetime(TimeSpan.FromDays(Cx.KeyLifetimeDays));

      var serviceProvider = services.BuildServiceProvider();
      var ProtectorProvider = serviceProvider.GetRequiredService<IDataProtectionProvider>();
      ICredentialEncryptionService credentialEncryptionService = new CredentialEncryptionService(ProtectorProvider);

      string originalData = "SensitiveInformation123!";
      string protectedData = credentialEncryptionService.Encrypt(originalData);
      string unprotectedData = credentialEncryptionService.Decrypt(protectedData);
      Assert.Equal(originalData, unprotectedData);
    }



  }
}