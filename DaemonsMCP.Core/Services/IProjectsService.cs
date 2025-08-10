using DaemonsMCP.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Services {
  public interface IProjectsService {
    Task<IEnumerable<ProjectModel>> GetProjectsAsync();
  }
}
