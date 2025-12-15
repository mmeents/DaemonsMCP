using System.Security.Cryptography;
using DaemonsMCP.Domain.Extensions;

namespace DaemonsMCP.xUnit.Tests {
  public class UnitTest1 {
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

    

  }
}