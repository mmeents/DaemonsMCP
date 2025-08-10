using DaemonsMCP.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaemonsMCP.Core.Config;

namespace DaemonsMCP.Core.Services {
  public class ProjectService : IProjectsService {
    private readonly IAppConfig _config;

    public ProjectService( IAppConfig config)
    {
      _config = config;
    }
    public Task<IEnumerable<ProjectModel>> GetProjectsAsync() {
      return Task.FromResult(_config.Projects.Values.AsEnumerable());
    }
  }
}
