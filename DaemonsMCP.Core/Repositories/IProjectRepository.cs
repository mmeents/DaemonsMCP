using DaemonsMCP.Core.Extensions;
using DaemonsMCP.Core.Models;
using Microsoft.Extensions.Logging;
using PackedTables.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Repositories {
  public interface IProjectRepository {
    public string ProjectsFilePathName { get; }
    public PackedTableSet ProjectsTableSet { get; }
    public TableModel Projects { get; set; }

    public event Action OnProjectsLoadedEvent;
    public void Save();
    public void Load();

    public ProjectModel AddProject(ProjectModel project) ;

    public ProjectModel UpdateProject(ProjectModel project);

    public bool DeleteProject(int projectId) ;

    public List<ProjectModel> GetAllProjects();

  }
}
