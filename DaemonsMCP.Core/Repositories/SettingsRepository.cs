using DaemonsMCP.Core.Config;
using DaemonsMCP.Core.Extensions;
using DaemonsMCP.Core.Models;
using DaemonsMCP.Core.Services;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Logging;
using PackedTables.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace DaemonsMCP.Core.Repositories {
  public class SettingsRepository : ISettingsRepository, IDisposable {
    private volatile bool _isDisposed = false;

    private readonly ILogger<SettingsRepository> _logger;
    public string SettingsFilePathName { get; set; } = string.Empty;
    public PackedTableSet SettingsTableSet { get; private set; } = new PackedTableSet();

    public TableModel SettingsTbl { get; set; }

    private RepositoryFileWatcher _watcher;

    public SettingsRepository(ILoggerFactory loggerFactory) { 
      _logger = loggerFactory.CreateLogger<SettingsRepository>();
      SettingsFilePathName = System.IO.Path.Combine(Sx.CommonAppPath, Cx.SettingsTblFileName);
      SettingsTableSet.LoadFromFile(SettingsFilePathName);
      SettingsTbl = SettingsTableSet[Cx.SettingsTbl] ?? MakeSettingsTable();

      // Setup file watcher
      _watcher = new RepositoryFileWatcher(
        SettingsFilePathName,
        () => Load(), // Reload when file changes
        loggerFactory
      );

      _logger.LogDebug($"🚀 SettingsRepository started ");
    }

    public void Dispose() {
      if (_isDisposed) return;
      _logger.LogInformation("🛑 Project Repo Disposing.");
      _isDisposed = true;
      _watcher.Dispose();
    }

    public event Action OnSettingsLoadedEvent = delegate { };
    private void DoOnSettingsLoadedEvent() {
      if (OnSettingsLoadedEvent != null) {
        OnSettingsLoadedEvent();
      }
    }

    private TableModel MakeSettingsTable() {
      if (SettingsTableSet[Cx.SettingsTbl] != null) {
        SettingsTableSet.RemoveTable(Cx.SettingsTbl);
      }
      var table = SettingsTableSet.AddTable(Cx.SettingsTbl);
      SettingsTbl = table;
      table.AddColumn(Cx.SettingNameCol, ColumnType.String);
      table.AddColumn(Cx.SettingValueCol, ColumnType.String);

      List<string> BlockedExt = new List<string> { ".env",".key", ".pem", ".p12", ".pfx", ".crt", ".cer", ".db", ".sqlite", ".sqlite3" };
      BlockedExtensions = BlockedExt;

      List<string> blockedFolders = new List<string> { "node_modules", "bin", "obj", ".git", ".svn", ".hg", ".git", ".vs", "node_modules", ".ssh" };
      BlockedFolders = blockedFolders;

      List<string> blockedFileNames = new List<string> { ".env.local", ".env.production", ".env.development", "appsettings.json", 
        "appsettings.Development.json", "appsettings.Production.json","web.config", "app.config","secrets.json", "user-secrets.json",
        ".gitignore","id_rsa", "id_ed25519","database.db", };
      BlockedFileNames = blockedFileNames;

      return table;
    }

    public void Save() {
      SettingsTableSet.SaveToFile(SettingsFilePathName);
    }

    public void Load() {
      SettingsTableSet = new PackedTableSet();
      SettingsTableSet.LoadFromFile(SettingsFilePathName);
      SettingsTbl = SettingsTableSet[Cx.SettingsTbl] ?? MakeSettingsTable();
      DoOnSettingsLoadedEvent();
    }

    public VersionSettings GetVersionSettings() { 
        var logLevel = GetSetting(Cx.LogLevelSetting);
        if (string.IsNullOrEmpty( logLevel)) {
          logLevel = "Information";
        }
        var nodesFilePath = GetSetting(Cx.NodesFilePathSetting);
        if (string.IsNullOrEmpty(nodesFilePath)) {
          nodesFilePath = Sx.CommonAppPath;
        }
        var settings = new VersionSettings {
            Name = Cx.AppName,
            Version = Cx.AppVersion,
            LogLevel = logLevel,
            NodesFilePath = nodesFilePath
        };
        return settings;
    }

    public SecuritySettings GetSecuritySettings() {
      var settings = new SecuritySettings {
        AllowedExtensions = this.AllowedExtensions,
        BlockedExtensions = this.BlockedExtensions,
        WriteProtectedPaths = this.BlockedFolders,
        BlockedFileNames = this.BlockedFileNames,
        MaxFileSize = this.MaxFileSize,
        AllowWrite = this.AllowFileWrites,
        MaxFileWriteSize = this.MaxFileWriteSize
      };
      return settings;
    }

    public string GetSetting(string name) {
      if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));      
      if ( SettingsTbl.FindFirst(Cx.SettingNameCol, name)) {
        string value = SettingsTbl.Current[Cx.SettingValueCol].ValueString ?? string.Empty;
        return value;
      }      
      return "";
    }

    public void SetSetting(string name, string value) {
      if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
      if (SettingsTbl.FindFirst(Cx.SettingNameCol, name)) {
        SettingsTbl.Current[Cx.SettingValueCol].ValueString = value;
      } else {
        var row = SettingsTbl.AddRow();
        row[Cx.SettingNameCol].ValueString = name;
        row[Cx.SettingValueCol].ValueString = value;
        SettingsTbl.Post();
      }
    }



    public List<string> AllowedExtensions {
      get {
        var setting = GetSetting(Cx.AllowedExtensionsSetting);
        if (string.IsNullOrEmpty(setting)) return new List<string>();
        return setting.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
      }
      set {
        if (value == null) throw new ArgumentNullException(nameof(value));
        var setting = string.Join(',', value);
        SetSetting(Cx.AllowedExtensionsSetting, setting);
      }
    }

    public List<string> BlockedExtensions {
      get {
        var setting = GetSetting(Cx.BlockedExtensionsSetting);
        if (string.IsNullOrEmpty(setting)) return new List<string>();
        return setting.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
      }
      set {
        if (value == null) throw new ArgumentNullException(nameof(value));
        var setting = string.Join(',', value);
        SetSetting(Cx.BlockedExtensionsSetting, setting);
      }
    }

    public List<string> BlockedFolders {
      get {
        var setting = GetSetting(Cx.BlockedFolderSettings);
        if (string.IsNullOrEmpty(setting)) return new List<string>();
        return setting.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
      }
      set {
        if (value == null) throw new ArgumentNullException(nameof(value));
        var setting = string.Join(',', value);
        SetSetting(Cx.BlockedFolderSettings, setting);
      }
    }

    public List<string> BlockedFileNames {
      get {
        var setting = GetSetting(Cx.BlockedFileNameSettings);
        if (string.IsNullOrEmpty(setting)) return new List<string>();
        return setting.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
      }
      set {
        if (value == null) throw new ArgumentNullException(nameof(value));
        var setting = string.Join(',', value);
        SetSetting(Cx.BlockedFileNameSettings, setting);
      }
    }

    public long MaxFileSizeBytes {
      get {
        var setting = GetSetting(Cx.MaxFileSizeSetting);
        if (string.IsNullOrEmpty(setting)) return 50 * 1024;  // 50KB default
        try { 
          var size = setting.ParseFileSize();
          return size;
        } catch (Exception ex) {
          _logger.LogError(ex, $"Error parsing MaxFileSize setting: '{setting}'. Defaulting to 50KB.");
          return 50 * 1024;
        }        
      }     
    }

    public string MaxFileSize {
      get {
        var size = GetSetting(Cx.MaxFileSizeSetting);
        if (string.IsNullOrEmpty(size)) return "50KB";
        return size;
      }
      set {
        if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));
        try {
          var size = value.ParseFileSize(); // Validate
        } catch (Exception ex) {
          throw new ArgumentException($"Invalid file size format: '{value}'. Expected formats like '100 KB', '1 MB', '2 GB'.", ex);
        }
        SetSetting(Cx.MaxFileSizeSetting, value);
      }
    }

    public long MaxFileWriteSizeBytes {
      get {
        var setting = GetSetting(Cx.MaxFileWriteSizeSetting);
        if (string.IsNullOrEmpty(setting)) return 500 * 1024;  // 0.5MB default
        try { 
          var size = setting.ParseFileSize();
          return size;
        } catch (Exception ex) {
          _logger.LogError(ex, $"Error parsing MaxFileWriteSize setting: '{setting}'. Defaulting to 500KB.");
          return 500 * 1024;
        }        
      }
    }

    public string MaxFileWriteSize {
      get {
        var size = GetSetting(Cx.MaxFileWriteSizeSetting);
        if (string.IsNullOrEmpty(size)) return "500KB";
        return size;
      }
      set {
        if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));
        try {
          var size = value.ParseFileSize(); // Validate
        } catch (Exception ex) {
          throw new ArgumentException($"Invalid file size format: '{value}'. Expected formats like '100 KB', '1 MB', '2 GB'.", ex);
        }
        SetSetting(Cx.MaxFileWriteSizeSetting, value);
      }
    }

    public bool AllowFileWrites {
      get {
        var setting = GetSetting(Cx.AllowFileWritesSetting);
        if (string.IsNullOrEmpty(setting)) return false;
        return setting.Equals("true", StringComparison.OrdinalIgnoreCase) || setting == "1";
      }
      set {
        SetSetting(Cx.AllowFileWritesSetting, value ? "true" : "false");
      }
    }


  }
}
