using DaemonsMCP.Core.Extensions;
using DaemonsMCP.Core.Models;
using DaemonsMCP.Core.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Services {
  public class ProjectTblWatchService : IDisposable {
    private volatile bool _isDisposed = false;
    private readonly ILogger<ProjectIndexWatchService> _logger;
    private FileSystemWatcher? _watcher;
    private IProjectRepository _projectRepository;
    private string ProjectRootPath = string.Empty;
    private System.Threading.Timer? _debounceTimer;
    private readonly Lock _debounceLock = new();

    public DateTime LastModified { get; private set; } = DateTime.MinValue;

    public ProjectTblWatchService(ILoggerFactory loggerFactory, IProjectRepository projectRepository) { 
        _logger = loggerFactory?.CreateLogger<ProjectIndexWatchService>() ?? throw new ArgumentNullException(nameof(loggerFactory), "LoggerFactory cannot be null");
        _logger.LogDebug($"🚀 ProjectTblWatchService started ");
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository), "ProjectRepository cannot be null");
        StartWatching();
    }

    public void Dispose() { 
      if (_isDisposed) return;
      StopWatching();
      StopDebounceTimer();
      _isDisposed = true;
      _logger.LogDebug($"❌ ProjectTblWatchService Disposed.");
    }

    public void StartWatching() {    
      var ProjectFullPath = _projectRepository.ProjectsFilePathName;
      ProjectRootPath = Path.GetDirectoryName(ProjectFullPath);
      if (!Directory.Exists(ProjectRootPath)) return;
      if (_watcher != null) {
        if (_watcher.EnableRaisingEvents) return; // Already watching
        _watcher.EnableRaisingEvents = true;
        return;
      }

      _watcher = new FileSystemWatcher(ProjectRootPath, Cx.ProjectsTblFileName) {
        IncludeSubdirectories = true,
        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size
      };
      _watcher.Created += OnFileEvent;
      _watcher.Changed += OnFileEvent;
      _watcher.Deleted += OnFileEvent;
      _watcher.Renamed += OnFileRenamed;      
      _watcher.Error += OnWatcherError;
      _watcher.EnableRaisingEvents = true;
      _logger.LogDebug($"👁️ Started watching: {ProjectRootPath}");
    }

    public void StopWatching() {
      if (_watcher != null) {
        _watcher.EnableRaisingEvents = false;
        _watcher.Created -= OnFileEvent;
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
      LastModified = DateTime.UtcNow;
      StartDebounceTimer(500);
      _logger.LogDebug($"⚡ File Change Detected: {e.Name} - {e.ChangeType}");
    }

    private void OnFileRenamed(object sender, RenamedEventArgs e) {
      if (_isDisposed) return;
      LastModified = DateTime.UtcNow;
      StartDebounceTimer(500);
      _logger.LogDebug($"⚡ Rename Change Detected: {e.Name}");
    }

    private void OnWatcherError(object sender, ErrorEventArgs e) {
      _logger.LogError($"❌ File watcher error for {ProjectRootPath}: {e.GetException().Message}");

      // Try to restart the watcher
      try {
        _watcher?.Dispose();
        StartWatching();
      } catch (Exception ex) {
        _logger.LogError($"❌ Failed to restart watcher: {ex.Message}");
      }
    }

    public void StartDebounceTimer(int milliseconds) {
      if (_isDisposed) return;
      lock (_debounceLock) {      
        if (_debounceTimer != null) {
            _debounceTimer.Change(milliseconds, Timeout.Infinite);
        } else {
            _debounceTimer = new System.Threading.Timer(DebounceTimerElapsed, null, milliseconds, Timeout.Infinite);
        }
      }
    }

    public void StopDebounceTimer() {
      if (_isDisposed) return;
      lock (_debounceLock) {
        if (_debounceTimer != null) {
            _debounceTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _debounceTimer.Dispose();
            _debounceTimer = null;
        }
      }
    }

    private void DebounceTimerElapsed(object? state) {
      if (_isDisposed) return;
        StopDebounceTimer();
      try { 

        _projectRepository.Load();
        _logger.LogDebug($"✅ Projects table reloaded after debounce.");

      }
      catch (Exception ex) { 
        _logger.LogError($"❌ Error in DebounceTimerElapsed: {ex.Message}");
      }

    }

  
  }
}
