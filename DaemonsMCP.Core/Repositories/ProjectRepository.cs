using DaemonsMCP.Core.Extensions;
using DaemonsMCP.Core.Models;
using DaemonsMCP.Core.Services;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using PackedTables.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Repositories {
  public class ProjectRepository : IProjectRepository, IDisposable {
    private volatile bool _isDisposed = false;
    private readonly ILogger<ProjectRepository> _logger;
    public string ProjectsFilePathName { get; set; } = "";
    public ProjectTblWatchService? WatchService { get; set; }
    public PackedTableSet ProjectsTableSet { get; private set; } = new PackedTableSet();
    public TableModel Projects { get; set;}
    public ProjectRepository(ILoggerFactory loggerFactory) {
      _logger = loggerFactory.CreateLogger<ProjectRepository>();
      ProjectsFilePathName = Path.Combine(Sx.CommonAppPath,  Cx.ProjectsTblFileName);

      ProjectsTableSet.LoadFromFile(ProjectsFilePathName);
      Projects = ProjectsTableSet["Projects"] ?? MakeProjectsTable();

      WatchService = new ProjectTblWatchService(loggerFactory, this);
    }

    public event Action OnProjectsLoadedEvent = delegate { };

    private void DoOnProjectsLoadedEvent() {
      if (OnProjectsLoadedEvent != null) {
        OnProjectsLoadedEvent();
      }
    }

    public TableModel MakeProjectsTable() {
      if (ProjectsTableSet[Cx.ProjectsTbl]!= null) { 
        ProjectsTableSet.RemoveTable( Cx.ProjectsTbl);
      }
      var table = ProjectsTableSet.AddTable(Cx.ProjectsTbl);      
      table.AddColumn(Cx.ProjectNameCol, ColumnType.String);
      table.AddColumn(Cx.ProjectDescriptionCol, ColumnType.String);
      table.AddColumn(Cx.ProjectPathCol, ColumnType.String);      

      return table;
    }

    public void Save() {
      ProjectsTableSet.SaveToFile(ProjectsFilePathName);
    }

    public void Load() {
      ProjectsTableSet.LoadFromFile(ProjectsFilePathName);
      Projects = ProjectsTableSet["Projects"] ?? MakeProjectsTable();
      DoOnProjectsLoadedEvent();
    }

    public ProjectModel AddProject(ProjectModel project) {
      if (project == null) throw new ArgumentNullException(nameof(project));
      if (string.IsNullOrEmpty(project.Name)) throw new ArgumentNullException(nameof(project.Name));
      if (string.IsNullOrEmpty(project.Path)) throw new ArgumentNullException(nameof(project.Path));
      if (Projects.Rows.Any(r => r.Value[Cx.ProjectNameCol]?.ValueString?.Equals(project.Name, StringComparison.OrdinalIgnoreCase) == true)) {
        throw new InvalidOperationException($"Project with name '{project.Name}' already exists.");
      }
      var newRow = Projects.AddRow();
      project.Id = newRow.Id;
      newRow[Cx.ProjectNameCol].ValueString = project.Name;
      newRow[Cx.ProjectDescriptionCol].ValueString = project.Description;
      newRow[Cx.ProjectPathCol].ValueString = project.Path;
      Projects.Post();      
      return project;
    }

    public ProjectModel UpdateProject(ProjectModel project) {
      if (project == null) throw new ArgumentNullException(nameof(project));
      if (string.IsNullOrEmpty(project.Name)) throw new ArgumentNullException(nameof(project.Name));
      if (string.IsNullOrEmpty(project.Path)) throw new ArgumentNullException(nameof(project.Path));
      var hasRow =Projects.Rows.ContainsKey(project.Id);
      if (!hasRow ) {
        throw new ArgumentException($"Project with ID '{project.Id}' does not exist.");
      }
      Projects.FindFirst(Cx.ProjectIdCol, project.Id);
      Projects.Edit();
      var row = Projects.Current;
      row[Cx.ProjectNameCol].ValueString = project.Name;
      row[Cx.ProjectDescriptionCol].ValueString = project.Description;
      row[Cx.ProjectPathCol].ValueString = project.Path;
      Projects.Post();
      return project;
    }

    public bool DeleteProject(int projectId) {
      var hasRow = Projects.Rows.ContainsKey(projectId);
      if (!hasRow) {
        throw new ArgumentException($"Project with ID '{projectId}' does not exist.");
      }
      if ( Projects.FindFirst(Cx.ProjectIdCol, projectId)) {
        Projects.DeleteCurrentRow();
      }
      return true;
    }

    public List<ProjectModel> GetAllProjects() {
      var projectList = new List<ProjectModel>();
      try { 
        foreach (var row in Projects.Rows.Values) {
          var project = new ProjectModel {
            Id = row.Id,
            Name = row[Cx.ProjectNameCol].ValueString ?? "",
            Description = row[Cx.ProjectDescriptionCol].ValueString ?? "",
            Path = row[Cx.ProjectPathCol].ValueString ?? ""
          };
          projectList.Add(project);
        }
        return projectList;
      } catch (Exception ex) {
        _logger.LogError(ex, "Error getting all projects");
        return projectList;
      }

    }

    public void Dispose() {
      if (_isDisposed) return;
      _logger.LogInformation("🛑 Project Repo Disposing.");
      _isDisposed = true;
      WatchService?.Dispose();

    }
  }
}
