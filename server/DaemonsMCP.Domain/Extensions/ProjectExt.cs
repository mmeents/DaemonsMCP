using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaemonsMCP.Domain.Entities;

namespace DaemonsMCP.Domain.Extensions {
  public static class ProjectExt {
    public static string CopyToBackup(this Project project, string filePath) {
      if (project == null) throw new ArgumentNullException(nameof(project));
      if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));
      string resolvedFilePath = Path.GetFullPath(filePath);
      if (!resolvedFilePath.StartsWith(project.RootPath, StringComparison.OrdinalIgnoreCase)) {
        throw new ArgumentException("File path is not within the project directory", nameof(filePath));
      }
      var backupPath = filePath + $".backup.{DateTime.Now:yyyyMMdd_HHmmss}";
      File.Copy(resolvedFilePath, backupPath, true);
      return backupPath;
    }

  }
}
