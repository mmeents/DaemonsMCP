using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Infrastructure.Services {

  public class ProjectFileWatcherService : IDisposable {
    private readonly int _projectId;
    private readonly string _projectPath;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ProjectFileWatcherService> _logger;
    private FileSystemWatcher? _watcher;
    private System.Threading.Timer? _debounceTimer;
    private readonly ConcurrentQueue<FileChangeEvent> _pendingChanges = new();
    private readonly Lock _timerLock = new();
    private bool _isDisposed;

    // Event to notify IndexingService to wake up
    public event EventHandler<int>? IndexingRequested; // passes projectId

    public ProjectFileWatcherService(
        int projectId,
        string projectPath,
        IServiceScopeFactory scopeFactory,
        ILogger<ProjectFileWatcherService> logger) {
      _projectId = projectId;
      _projectPath = projectPath;
      _scopeFactory = scopeFactory;
      _logger = logger;
    }

    public void StartWatching() {
      if (_watcher != null) return;

      _watcher = new FileSystemWatcher(_projectPath, "*.cs") {
        IncludeSubdirectories = true,
        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size
      };

      _watcher.Changed += OnFileEvent;
      _watcher.Created += OnFileEvent;
      _watcher.Deleted += OnFileEvent;
      _watcher.Renamed += OnFileRenamed;
      _watcher.Error += OnWatcherError;

      _watcher.EnableRaisingEvents = true;
      _logger.LogDebug("👁️ Started watching: {ProjectPath}", _projectPath);
    }

    public void StopWatching() {
      if (_watcher != null) {
        _watcher.EnableRaisingEvents = false;
        _watcher.Dispose();
        _watcher = null;
        _logger.LogDebug("🛑 Stopped watching: {ProjectPath}", _projectPath);
      }
      StopDebounceTimer();
    }

    private void OnFileEvent(object sender, FileSystemEventArgs e) {
      if (_isDisposed || !e.FullPath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
        return;

      _pendingChanges.Enqueue(new FileChangeEvent {
        FilePath = e.FullPath,
        ChangeType = e.ChangeType,
        Timestamp = DateTime.UtcNow
      });

      // Start/restart debounce timer
      RestartDebounceTimer();

      _logger.LogDebug("⚡ Queued {ChangeType}: {FileName}", e.ChangeType, e.Name);
    }

    private void OnFileRenamed(object sender, RenamedEventArgs e) {
      if (_isDisposed) return;

      // Handle as delete old + create new
      _pendingChanges.Enqueue(new FileChangeEvent {
        FilePath = e.OldFullPath,
        ChangeType = WatcherChangeTypes.Deleted,
        Timestamp = DateTime.UtcNow
      });

      _pendingChanges.Enqueue(new FileChangeEvent {
        FilePath = e.FullPath,
        ChangeType = WatcherChangeTypes.Created,
        Timestamp = DateTime.UtcNow
      });

      RestartDebounceTimer();
    }

    private void RestartDebounceTimer() {
      lock (_timerLock) {
        // Stop existing timer
        _debounceTimer?.Dispose();

        // Start new timer - fires after 2 seconds of no changes
        _debounceTimer = new System.Threading.Timer(
            async _ => await FlushChangesToQueueAsync(),
            null,
            TimeSpan.FromSeconds(2),
            Timeout.InfiniteTimeSpan // one-shot timer
        );
      }
    }

    private void StopDebounceTimer() {
      lock (_timerLock) {
        _debounceTimer?.Dispose();
        _debounceTimer = null;
      }
    }

    private async Task FlushChangesToQueueAsync() {
      if (_isDisposed) return;

      StopDebounceTimer();

      var changes = new List<FileChangeEvent>();
      while (_pendingChanges.TryDequeue(out var change)) {
        changes.Add(change);
      }

      if (changes.Count == 0) return;

      // Deduplicate - keep latest change per file
      var latestChanges = changes
          .GroupBy(c => c.FilePath, StringComparer.OrdinalIgnoreCase)
          .Select(g => g.OrderByDescending(c => c.Timestamp).First())
          .ToList();

      _logger.LogInformation("📝 Flushing {Count} file changes to IndexQueue", latestChanges.Count);

      // Create a new scope for this entire operation
      using var scope = _scopeFactory.CreateScope();
      var fileSystemNodeRepository = scope.ServiceProvider.GetRequiredService<IFileSystemNodeRepository>();
      var indexQueueRepository = scope.ServiceProvider.GetRequiredService<IIndexQueueRepository>();

      // Insert into IndexQueue table
      foreach (var change in latestChanges) {
        try {
          // Get or create FileSystemNode
          var relativePath = Path.GetRelativePath(_projectPath, change.FilePath);

          // Handle deletions
          if (change.ChangeType == WatcherChangeTypes.Deleted) {
            var existing = await fileSystemNodeRepository.GetByPathAsync(
                _projectId,
                relativePath);

            if (existing != null) {
              IndexQueue item2 = IndexQueue.Create(_projectId, existing.Id, relativePath);
              await indexQueueRepository.AddAsync(item2);              
            }
            continue; // Don't try to create for deleted files
          }

          // Determine if it's a file or directory
          bool isDirectory = Directory.Exists(change.FilePath);
          long? fileSize = null;

          if (!isDirectory && File.Exists(change.FilePath)) {
            var fileInfo = new FileInfo(change.FilePath);
            fileSize = fileInfo.Length;
          }

          // Get or create FileSystemNode (recursively creates parent directories)
          var fileSystemNode = await fileSystemNodeRepository.GetOrCreateAsync(
              _projectId,
              relativePath,
              isDirectory,
              fileSize);

          // Create IndexQueue entry
          IndexQueue item = IndexQueue.Create( _projectId, fileSystemNode.Id, relativePath);
          await indexQueueRepository.AddAsync(item);

        } catch (Exception ex) {
          _logger.LogError(ex, "Failed to enqueue change for {FilePath}", change.FilePath);
        }
      }

      // Emit event to wake up IndexingService
      IndexingRequested?.Invoke(this, _projectId);

      _logger.LogDebug("✅ Emitted IndexingRequested event for project {ProjectId}", _projectId);
    }

    private void OnWatcherError(object sender, ErrorEventArgs e) {
      _logger.LogError(e.GetException(), "File watcher error for {ProjectPath}", _projectPath);

      try {
        StopWatching();
        StartWatching();
      } catch (Exception ex) {
        _logger.LogError(ex, "Failed to restart watcher for {ProjectPath}", _projectPath);
      }
    }

    public void Dispose() {
      if (_isDisposed) return;
      _isDisposed = true;

      StopWatching();
      _logger.LogDebug("❌ ProjectFileWatcherService disposed for project {ProjectId}", _projectId);
    }
  }

  public class FileChangeEvent {
    public string FilePath { get; set; } = string.Empty;
    public WatcherChangeTypes ChangeType { get; set; }
    public DateTime Timestamp { get; set; }
  }
}
