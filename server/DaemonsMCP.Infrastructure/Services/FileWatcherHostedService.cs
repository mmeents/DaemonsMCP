using DaemonsMCP.Application.FileSystem.Services;
using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Repositories;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DaemonsMCP.Infrastructure.Services {

  public class FileWatcherHostedService : BackgroundService {
    private readonly IServiceProvider _serviceProvider;
    private readonly IProjectFileWatcherFactory _watcherFactory;
    private readonly ILogger<FileWatcherHostedService> _logger;
    private readonly Dictionary<int, ProjectFileWatcherService> _watchers = new();
    private readonly Lock _watchersLock = new();

    public FileWatcherHostedService(
        IServiceProvider serviceProvider,
        IProjectFileWatcherFactory watcherFactory,        
        ILogger<FileWatcherHostedService> logger) 
    {
      _serviceProvider = serviceProvider;
      _watcherFactory = watcherFactory;      
      _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
      _logger.LogInformation("🚀 FileWatcherHostedService starting");

      // Wait a bit for app to fully start
      await Task.Delay(2000, stoppingToken);

      // Load all projects and start watchers
      List<DaemonsMCP.Domain.Entities.Project> projects;
      using (var scope = _serviceProvider.CreateScope()) {
        var projectRepository = scope.ServiceProvider.GetRequiredService<IProjectRepository>();
        projects = await projectRepository.GetAllAsync(stoppingToken);
      }

      foreach (var project in projects) {   
        if (project != null) { 
          StartWatcherForProject(project, stoppingToken);
        }
      }

      _logger.LogInformation("👁️ Watching {Count} projects", _watchers.Count);

      await Task.Delay(1000, stoppingToken);

      // Initial sync and indexing for all projects
      foreach (var project in projects) {
        if (project != null) {
          await RunIndexingForProjectAsync(project.Id, stoppingToken);     
        }
      }

      await Task.Delay(Timeout.Infinite, stoppingToken);

    }

    /// <summary>
    /// Create a NEW scope for each indexing run - this prevents DbContext concurrency issues!
    /// </summary>
    private async Task RunIndexingForProjectAsync(int projectId, CancellationToken cancellationToken) {      
      try {
        // 🎯 KEY FIX: Create NEW scope for THIS operation
        using var scope = _serviceProvider.CreateScope();
        var fileSystemSyncService = scope.ServiceProvider.GetRequiredService<IFileSystemSyncService>();
        var indexingService = scope.ServiceProvider.GetRequiredService<IIndexingService>();
        var projectRepository = scope.ServiceProvider.GetRequiredService<IProjectRepository>();
        
        var project = await projectRepository.GetByIdAsync(projectId, cancellationToken);
        if (project == null) {
          _logger.LogWarning("Project {ProjectId} not found", projectId);
          return;
        }

        await fileSystemSyncService.SyncProjectAsync(project, cancellationToken);
        await indexingService.RunAsync(project.Id, cancellationToken);
        
        _logger.LogDebug("✅ Completed indexing for project {ProjectId}", projectId);
      } catch (Exception ex) {
        _logger.LogError(ex, "❌ Error running indexing for project {ProjectId}", projectId);
      }
      // Scope disposes here - DbContext gets cleaned up!
    }

    private void StartWatcherForProject(
        DaemonsMCP.Domain.Entities.Project project,        
        CancellationToken cancellationToken) {
      lock (_watchersLock) {
        if (_watchers.ContainsKey(project.Id)) {
          _logger.LogWarning("Watcher already exists for project {ProjectId}", project.Id);
          return;
        }
      }

      // Use factory to create watcher
      var watcher = _watcherFactory.Create(project.Id, project.RootPath);

      // Subscribe to IndexingRequested event
      watcher.IndexingRequested += (sender, projectId) => {
        _logger.LogDebug("⚡ Indexing requested for project {ProjectId}", projectId);
        // Fire and forget - each call creates its own scope
        _ = Task.Run(async () => {
          await RunIndexingForProjectAsync(projectId, cancellationToken);
        });
      };

      watcher.StartWatching();

      lock (_watchersLock) {
        _watchers[project.Id] = watcher;
      }

      _logger.LogInformation("✅ Started watcher for project {ProjectName} (ID: {ProjectId})",
          project.Name, project.Id);
    }

    public override async Task StopAsync(CancellationToken cancellationToken) {
      _logger.LogInformation("🛑 Stopping FileWatcherHostedService");

      lock (_watchersLock) {
        foreach (var watcher in _watchers.Values) {
          watcher.Dispose();
        }
        _watchers.Clear();
      }

      await base.StopAsync(cancellationToken);
    }
  }

}
