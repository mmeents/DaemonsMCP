using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Services {
  public class RepositoryFileWatcher : IDisposable {
    private volatile bool _isDisposed = false;
    private string _filePath;
    private FileSystemWatcher? _watcher;
    private readonly ILogger<RepositoryFileWatcher> _logger;
    private readonly Action _onChanged;
    private DateTime _lastEventTime = DateTime.MinValue;
    private const int DEBOUNCE_MS = 500; // Prevent duplicate events
    private System.Threading.Timer? _debounceTimer;
    private readonly Lock _debounceLock = new();

    public RepositoryFileWatcher(string filePath, Action onChanged, ILoggerFactory loggerFactory) {
      _filePath = filePath;
      _onChanged = onChanged;
      _logger = loggerFactory.CreateLogger<RepositoryFileWatcher>();
      Start();
    }

    public void Dispose() {
      if (_watcher == null) return;
      _watcher.Changed -= OnFileChanged;
      _watcher.EnableRaisingEvents = false;
      _watcher.Dispose();
      _watcher = null;
      _logger.LogDebug("🛑 File watcher disposed");
    }

    public void Start() {
      if (_watcher != null) { 
        if (_watcher.EnableRaisingEvents) return; // Already watching        
        _watcher.EnableRaisingEvents = true;
        return;
      }
      var directory = Path.GetDirectoryName(_filePath)!;
      var fileName = Path.GetFileName(_filePath);

      _watcher = new FileSystemWatcher(directory, fileName) {
        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size,
        InternalBufferSize = 32 * 1024
      };

      _watcher.Changed += OnFileChanged;
      _watcher.EnableRaisingEvents = true;
      _logger.LogDebug($"📁 Watching {fileName} for changes");
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e) {
      if (_isDisposed) return;
      StartDebounceTimer(DEBOUNCE_MS);
      _logger.LogDebug($"⚡ File Change Detected: {e.Name} - {e.ChangeType}");
    }

    public void StartDebounceTimer(int milliseconds) {
      if (_isDisposed) return;
      lock (_debounceLock) {
        if (_debounceTimer == null) {          
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
        _onChanged?.Invoke();
        _logger.LogDebug($"✅ Projects table reloaded after debounce.");

      } catch (Exception ex) {
        _logger.LogError($"❌ Error in DebounceTimerElapsed: {ex.Message}");
      }

    }



  }
}
