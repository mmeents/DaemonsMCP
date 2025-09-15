using DaemonsMCP.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Config {
  public interface IAppConfig {
    VersionSettings Version { get; }
    IReadOnlyDictionary<string, ProjectModel> Projects { get; }
    bool IsConfigured { get; }
    SecuritySettings Security { get; }
    void Reload(string? configPath);
  }

}
