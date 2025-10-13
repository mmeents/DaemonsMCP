using DaemonsMCP.Core.Extensions;
using DaemonsMCP.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Services {

  public class ProjectIndexWatchService : IDisposable {    
    // This service is responsible for watching the project directory for changes
    public ProjectIndexWatchService(ILoggerFactory loggerFactory, ProjectIndexModel projectIndexModel, ISecurityService securityService) {      
      _logger = loggerFactory?.CreateLogger<ProjectIndexWatchService>() ?? throw new ArgumentNullException(nameof(loggerFactory), "LoggerFactory cannot be null");
      _projectIndexModel = projectIndexModel ?? throw new ArgumentNullException(nameof(projectIndexModel), "ProjectIndexModel cannot be null");
      _securityService = securityService ?? throw new ArgumentNullException(nameof(securityService), "SecurityService cannot be null");

      if (string.IsNullOrEmpty(_projectIndexModel.ProjectPath))
        throw new ArgumentException("Project root path cannot be null or empty", nameof(_projectIndexModel.ProjectPath));

      if (!Directory.Exists(_projectIndexModel.ProjectPath))
        throw new DirectoryNotFoundException($"Project root path does not exist: {_projectIndexModel.ProjectPath}");

      if (!Path.IsPathRooted(_projectIndexModel.ProjectPath))
        throw new ArgumentException("Project root path must be an absolute path", nameof(_projectIndexModel.ProjectPath));

       ProjectRootPath = _projectIndexModel.ProjectPath ?? throw new ArgumentNullException(nameof(_projectIndexModel.ProjectPath), "Project root path cannot be null");
       _logger.LogDebug($"🚀 Project {_projectIndexModel.ProjectName} Index Watch Service started ");
       StartupScanToFillQueue();
       StartWatching();       
    }

    public void Dispose() {
      if (_isDisposed) return;
      _isDisposed = true;
      if (_watcher != null) { 
        if (_watcher.EnableRaisingEvents) _watcher.EnableRaisingEvents = false;
        _watcher.Dispose();
      }
      _logger.LogDebug($"❌ ProjectIndexWatchService Disposed.");
    }

    private volatile bool _isDisposed = false;
    private readonly ILogger<ProjectIndexWatchService> _logger; 
    private readonly ISecurityService _securityService;
    private ProjectIndexModel _projectIndexModel;
    public string ProjectRootPath { get; private set; }
    private FileSystemWatcher? _watcher;
    private readonly ConcurrentQueue<FileChangeItem> _changeQueue = new();

    public ConcurrentQueue<FileChangeItem> ChangeQueue => _changeQueue;

    public void StartWatching() {
      if (!Directory.Exists(ProjectRootPath)) return;
      if (_watcher != null) {
        if (_watcher.EnableRaisingEvents) return; // Already watching
        _watcher.EnableRaisingEvents = true;
        return;
      }

      _watcher = new FileSystemWatcher(ProjectRootPath, "*.cs") {
        IncludeSubdirectories = true,
        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size
      };

   //   _watcher.Created += OnFileEvent;
      _watcher.Changed += OnFileEvent;
      _watcher.Deleted += OnFileEvent;
      _watcher.Renamed += OnFileRenamed;

      // Handle watcher errors gracefully
      _watcher.Error += OnWatcherError;

      _watcher.EnableRaisingEvents = _projectIndexModel?.IndexService?.Enabled?? false;

      if (Cx.IsDebug) _logger.LogDebug($"👁️ Started watching: {ProjectRootPath}");
    }

    public void StopWatching() {
      if (_watcher != null) {
        _watcher.EnableRaisingEvents = false;
    //    _watcher.Created -= OnFileEvent;
        _watcher.Changed -= OnFileEvent;
        _watcher.Deleted -= OnFileEvent;
        _watcher.Renamed -= OnFileRenamed;
        _watcher.Error -= OnWatcherError;
        _watcher.Dispose();
        _watcher = null;
        if (Cx.IsDebug) _logger.LogDebug($"🛑 Stopped watching: {ProjectRootPath}");
      }
    }

    private void OnFileEvent(object sender, FileSystemEventArgs e) {
      if (_isDisposed) return;

      // Quick filter at watcher level
      if (!e.FullPath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)) return;

      var changeItem = new FileChangeItem {
        FilePath = e.FullPath,
        ChangeType = e.ChangeType,
        Timestamp = DateTime.UtcNow
      };

      _changeQueue.Enqueue(changeItem);
      if (_projectIndexModel != null && _projectIndexModel.IndexService != null) { 
        _projectIndexModel.IndexService.StartTimer();
      }
      if (Cx.IsDebug) _logger.LogDebug($"⚡ Queued {e.ChangeType}: {e.Name}");
    }

    private void OnFileRenamed(object sender, RenamedEventArgs e) {
      if (_isDisposed) return;

      // Handle as delete old + create new
      _changeQueue.Enqueue(new FileChangeItem {
        FilePath = e.OldFullPath,
        ChangeType = WatcherChangeTypes.Deleted,
        Timestamp = DateTime.UtcNow
      });

      _changeQueue.Enqueue(new FileChangeItem {
        FilePath = e.FullPath,
        ChangeType = WatcherChangeTypes.Created,
        Timestamp = DateTime.UtcNow
      });
      if (_projectIndexModel != null && _projectIndexModel.IndexService != null) {
        _projectIndexModel.IndexService.StartTimer();
      }
      if (Cx.IsDebug) _logger.LogDebug($"⚡ Rename Change Enqued: {e.Name}");
    }

    private void OnWatcherError(object sender, ErrorEventArgs e) {
      _logger.LogError($"❌ File watcher error for {ProjectRootPath}: {e.GetException().Message}");

      // Try to restart the watcher
      try {
        StopWatching();
        StartWatching();
      } catch (Exception ex) {
        _logger.LogError($"❌ Failed to restart watcher: {ex.Message}");
      }
    }

    private void StartupScanToFillQueue() {
      if (_isDisposed) return;
      if (!Directory.Exists(ProjectRootPath)) return;
      var csFiles = Directory.GetFiles(ProjectRootPath, "*.cs", SearchOption.AllDirectories)
        .Where(file => _securityService.IsFileAllowed(file))
        .ToHashSet(StringComparer.OrdinalIgnoreCase);

      foreach (var file in csFiles) {
        _changeQueue.Enqueue(new FileChangeItem {
          FilePath = file,
          ChangeType = WatcherChangeTypes.Created,
          Timestamp = DateTime.UtcNow
        });
      }
      if (_projectIndexModel != null && _projectIndexModel.IndexService != null) {
        _projectIndexModel.IndexService.StartTimer();
      }
      if (Cx.IsDebug) _logger.LogDebug($"🔍 Startup scan queued {csFiles.Count} .cs files to index.");
    }


  }
}
