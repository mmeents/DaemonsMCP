using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaemonsMCP.Core.Config;
using DaemonsMCP.Core.Extensions;
using DaemonsMCP.Core.Models;
using DaemonsMCP.Core.Services;
using Microsoft.Extensions.Logging;
using PackedTables.Net;

namespace DaemonsMCP.Core.Repositories {
  public class IndexRepository : IIndexRepository {
    private readonly IAppConfig _appConfig;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<IndexRepository> _logger;
    private readonly IValidationService _validationService;
    private readonly ISecurityService _securityService;
    private readonly ConcurrentDictionary<string, ProjectIndexModel> _projectIndexs = new ConcurrentDictionary<string, ProjectIndexModel>();
    public IndexRepository(ILoggerFactory loggerFactory, IAppConfig appConfig, IValidationService validationService, ISecurityService securityService) {
      _appConfig = appConfig;
      _loggerFactory = loggerFactory;
      _appConfig.OnProjectsLoadedEvent += ProjectsReloaded;
      _logger = loggerFactory.CreateLogger<IndexRepository>() ?? throw new ArgumentNullException(nameof(loggerFactory));
      _validationService = validationService;
      _securityService = securityService;
      foreach (var project in _appConfig.Projects) {
        _projectIndexs[project.Key] = new ProjectIndexModel(_loggerFactory, project.Value, _validationService, _securityService);
      }
      
    }

    private void ProjectsReloaded() {
      
      foreach (var key in _projectIndexs.Keys) {
        if (!_appConfig.Projects.ContainsKey(key)) {
          if ( _projectIndexs.TryRemove(key, out var leavingProject)) { 
            _logger.LogInformation($"🗑️ Project '{key}' removed from index.");
            leavingProject.Dispose();
          }
        }
      }

      foreach (var project in _appConfig.Projects) {
        if (!_projectIndexs.ContainsKey(project.Key)) { 
          _projectIndexs[project.Key] = new ProjectIndexModel(_loggerFactory, project.Value, _validationService, _securityService);
        }
      }
      DoOnProjectsReLoadedEvent();
    }

    public event Action OnProjectsReLoadedEvent = delegate { };

    private void DoOnProjectsReLoadedEvent() {
      if (OnProjectsReLoadedEvent != null) {
        OnProjectsReLoadedEvent();
      }
    }
    public ProjectIndexModel? GetProjectIndex(string projectName) {
      if (_projectIndexs.TryGetValue(projectName, out var projectIndex)) {
        return projectIndex;
      }
      return null;
    }

    public async Task<List<ClassListing>> GetClassListingsAsync(string projectName, int pageNo, int itemsPerPage, string? namespaceFilter = null, string? classNameFilter = null ) {
      var projectIndex = GetProjectIndex(projectName);
      if (projectIndex == null) {
        throw new ArgumentException($"Project '{projectName}' not found.");
      }
      return await projectIndex.GetClassListingsAsync( pageNo, itemsPerPage, namespaceFilter, classNameFilter).ConfigureAwait(false);
    }

    public async Task<ClassContent> GetClassContentAsync(string projectName, int classId) { 
      var projectIndex = GetProjectIndex(projectName);
      if (projectIndex == null) {
        throw new ArgumentException($"Project '{projectName}' not found.");
      }
      return await projectIndex.GetClassContentById(classId);
    }

    public async Task<OperationResult> AddUpdateClassContentAsync(string projectName, ClassContent classContent) {
      var projectIndex = GetProjectIndex(projectName);
      if (projectIndex == null) {
        throw new ArgumentException($"Project '{projectName}' not found.");
      }
      return await projectIndex.AddUpdateClassAsync(classContent);
    }

    public async Task<List<MethodListing>> GetMethodListingsAsync(string projectName, int pageNo, int itemsPerPage, string? namespaceFilter = null, string? classNameFilter = null, string? methodNameFilter=null ) {
      var projectIndex = GetProjectIndex(projectName);
      if (projectIndex == null) {
        throw new ArgumentException($"Project '{projectName}' not found.");
      }
      return await projectIndex.GetMethodListingsAsync(pageNo, itemsPerPage, namespaceFilter, classNameFilter, methodNameFilter );
    }

    public async Task<MethodContent> GetMethodContentAsync(string projectName, int methodId) {
      var projectIndex = GetProjectIndex(projectName);
      if (projectIndex == null) {
        throw new ArgumentException($"Project '{projectName}' not found.");
      }
      return await projectIndex.GetMethodContentById(methodId);
    }

    public async Task<OperationResult> AddUpdateMethodAsync(string projectName, MethodContent methodContent) {
      var projectIndex = GetProjectIndex(projectName);
      if (projectIndex == null) {
        throw new ArgumentException($"Project '{projectName}' not found.");
      }
      return await projectIndex.AddUpdateMethodContent(methodContent);
    }

  }


}
