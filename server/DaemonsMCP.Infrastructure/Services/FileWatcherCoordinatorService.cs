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

  public class FileWatcherCoordinatorService : BackgroundService {
    private readonly IServiceProvider _serviceProvider;
    private readonly IProjectFileWatcherFactory _watcherFactory;
    private readonly ILogger<FileWatcherCoordinatorService> _logger;
    private readonly Dictionary<int, ProjectFileWatcherService> _watchers = new();
    private readonly Lock _watchersLock = new();

    public FileWatcherCoordinatorService(
        IServiceProvider serviceProvider,
        IProjectFileWatcherFactory watcherFactory,        
        ILogger<FileWatcherCoordinatorService> logger) 
    {
      _serviceProvider = serviceProvider;
      _watcherFactory = watcherFactory;      
      _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
      _logger.LogInformation("🚀 FileWatcherCoordinatorService starting");

      // Wait a bit for app to fully start
      await Task.Delay(2000, stoppingToken);

      using var scope = _serviceProvider.CreateScope();
      var projectRepository = scope.ServiceProvider.GetRequiredService<IProjectRepository>();
      var indexingService = scope.ServiceProvider.GetRequiredService<IIndexingService>();
      var fileSystemSyncService = scope.ServiceProvider.GetRequiredService<IFileSystemSyncService>();

      // Load all projects and start watchers
      var projects = await projectRepository.GetAllAsync(stoppingToken);

      foreach (var project in projects) {   
        if (project != null) { 
          await StartWatcherForProjectAsync(project, indexingService, stoppingToken);
        }
      }

      _logger.LogInformation("👁️ Watching {Count} projects", _watchers.Count);

      await Task.Delay(1000, stoppingToken);

      // Initial sync and indexing for all projects
      foreach (var project in projects) {
        if (project != null) {
          try {
            await fileSystemSyncService.SyncProjectAsync(project, stoppingToken);
            await indexingService.RunAsync(project.Id, stoppingToken);
          } catch (Exception ex) {
            _logger.LogError(ex, "Error running indexing for project {ProjectId}", project.Id);
          }
        }
      }

      await Task.Delay(Timeout.Infinite, stoppingToken);

    }

    private async Task StartWatcherForProjectAsync(
        DaemonsMCP.Domain.Entities.Project project,
        IIndexingService indexingService,
        CancellationToken cancellationToken) {
      lock (_watchersLock) {
        if (_watchers.ContainsKey(project.Id)) {
          _logger.LogWarning("Watcher already exists for project {ProjectId}", project.Id);
          return;
        }
      }

      using var scope = _serviceProvider.CreateScope();
      var indexQueueRepository = scope.ServiceProvider.GetRequiredService<IIndexQueueRepository>();
      var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();

      // Use factory to create watcher with proper DI
      var watcher = _watcherFactory.Create(project.Id, project.RootPath);

      // Subscribe to IndexingRequested event
      watcher.IndexingRequested += (sender, projectId) => {
        _logger.LogDebug("⚡ Indexing requested for project {ProjectId}", projectId);     
      };

      watcher.StartWatching();

      lock (_watchersLock) {
        _watchers[project.Id] = watcher;
      }

      _logger.LogInformation("✅ Started watcher for project {ProjectName} (ID: {ProjectId})",
          project.Name, project.Id);
    }

    public override async Task StopAsync(CancellationToken cancellationToken) {
      _logger.LogInformation("🛑 Stopping FileWatcherCoordinatorService");

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
