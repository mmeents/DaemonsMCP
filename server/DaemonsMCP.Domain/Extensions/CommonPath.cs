using DaemonsMCP.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Extensions {
  public static class CommonPath {

    public static string ResolvePath(this string path) {
      // Handle relative paths
      if (!Path.IsPathRooted(path)) {
        // Relative to config file directory or current directory
        return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), path));
      }

      // Handle cross-platform path separators
      return Path.GetFullPath(path);
    }

    public static string CommonAppPath {
      get {
        string commonPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Cx.AppName).ResolvePath();
        if (!Directory.Exists(commonPath)) {
          Directory.CreateDirectory(commonPath);
        }
        return commonPath;
      }
    }

    public static string LogsAppPath {
      get {
        string logsPath = Path.Combine(CommonAppPath, "logs").ResolvePath();
        if (!Directory.Exists(logsPath)) {
          Directory.CreateDirectory(logsPath);
        }
        return logsPath;
      }
    }


  }
}
