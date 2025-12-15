using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Extensions {
  public static class TokenExt {
    public static string GenerateToken(int byteLength = 7) // Default to 7 for stronger tokens
   {
      if (byteLength < 5) throw new ArgumentException("Byte length too small for secure tokens.");

      var bytes = new byte[byteLength];
      RandomNumberGenerator.Fill(bytes);
      string token = Convert.ToBase64String(bytes)
          .TrimEnd('=')
          .Replace('+', '9')
          .Replace('/', '6')
          .ToUpper();

      return token;
    }
  }
}
