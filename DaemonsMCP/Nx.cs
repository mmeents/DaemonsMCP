using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP {
  // This class represents a collection of projects and their details.
  // its out of date.  you should use the DaemonsMcpConfiguration class to load projects from the configuration file.
  public static class Nx {
    // this was a hardcoded list of projects.  This was moved to the deamonsmcp.json configuration file.  
    public static IReadOnlyDictionary<string, Project> Projects { get; } = new Dictionary<string, Project> {
    };
  }
}
