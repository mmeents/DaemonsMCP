using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaemonsMCP.Core.Config;
using DaemonsMCP.Core.Extensions;
using MCPSharp;

namespace DaemonsMCP.Core {

  /// <summary>
  /// Project management tools for DaemonsMCP, note this out of date and unused. see DaemonsTools.cs for current tools.
  /// </summary>
  public class ProjectTools {
    private readonly AppConfig _config;
    public ProjectTools(AppConfig config) {
      _config = config ?? throw new ArgumentNullException(nameof(config), Cx.Dd1+Cx.ErrorNoConfig);
    }

    /// <summary>
    /// Gets list of available projects. A project is the name of the root folder.
    /// </summary>
    /// <returns>List of projects with names and descriptions</returns>
    [McpTool(Cx.ListProjectsCmd, Cx.ListProjectsDesc)]
    public object ListProjects() {
      return System.Text.Json.JsonSerializer.Serialize(new { 
        projects = _config.Projects.Values.ToArray() 
      });
    }

  }
}
