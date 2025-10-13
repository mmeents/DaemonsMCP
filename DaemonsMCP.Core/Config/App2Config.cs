using DaemonsMCP.Core.Models;
using DaemonsMCP.Core.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Config {
  public class App2Config : IAppConfig, IDisposable {
    private volatile bool _isDisposed = false;
    private readonly ILogger<App2Config> _logger;
    private readonly IProjectRepository _projectRepository;
    private readonly ISettingsRepository _settingsRepository;
    private SecuritySettings _securitySettings;
    private VersionSettings _versionSettings;
    private ConcurrentDictionary<string, ProjectModel> _projects = new ConcurrentDictionary<string, ProjectModel>(StringComparer.OrdinalIgnoreCase);
    public App2Config(
      ILoggerFactory loggerFactory, 
      IProjectRepository projectRepository, 
      ISettingsRepository settingsRepository
    ) {
      if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
      if (projectRepository == null) throw new ArgumentNullException(nameof(projectRepository));
      _logger = loggerFactory.CreateLogger<App2Config>();
      _projectRepository = projectRepository;
      _projectRepository.OnProjectsLoadedEvent += DoProjectsLoaded;
      _settingsRepository = settingsRepository;
      _settingsRepository.OnSettingsLoadedEvent += DoSettingsLoaded;
      _securitySettings = _settingsRepository.GetSecuritySettings();
      _versionSettings = _settingsRepository.GetVersionSettings();
      LoadProjects();
      _logger.LogDebug("🚀 AppConfig started.");
    }

    private void LoadProjects() {
      try { 
        _logger.LogInformation("🔄 Loading projects from repository.");

        _projects.Clear();
        var listProjets = _projectRepository.GetAllProjects();
        foreach (var project in listProjets) {
          _projects[project.Name] = project;
        }
        _logger.LogInformation($"✅ Loaded {_projects.Count} projects.");
      } catch (Exception ex) {
        _logger.LogError(ex, "❌ Error loading projects from repository.");
      }            
    }

    public void DoProjectsLoaded() {
      _logger.LogInformation("🔄 Projects reloaded via notification.");
      LoadProjects();
      DoOnProjectsLoadedEvent();
    }

    public void DoSettingsLoaded() {
      _logger.LogInformation("🔄 Settings reloaded via notification.");
      _securitySettings = _settingsRepository.GetSecuritySettings();
      _versionSettings = _settingsRepository.GetVersionSettings();
    }

    public event Action OnProjectsLoadedEvent = delegate { };

    private void DoOnProjectsLoadedEvent() {
      if (OnProjectsLoadedEvent != null) {
        OnProjectsLoadedEvent();
      }
    }

    public VersionSettings Version => _versionSettings;

    public IReadOnlyDictionary<string, ProjectModel> Projects => _projects;

    public bool IsConfigured => true;

    public SecuritySettings Security => _securitySettings;

    public void Reload() {
      LoadProjects();
    }

    public void Dispose() {
      if (_isDisposed) return;
      _logger.LogInformation("🛑 AppConfig Disposing.");
      _isDisposed = true;
      _projectRepository.OnProjectsLoadedEvent -= DoProjectsLoaded;      
      _settingsRepository.OnSettingsLoadedEvent -= DoSettingsLoaded;
      _projects.Clear();
      _projects = null!;      
    }
  }
  
}
