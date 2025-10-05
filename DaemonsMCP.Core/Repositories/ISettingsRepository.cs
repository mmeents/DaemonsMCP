using DaemonsMCP.Core.Extensions;
using DaemonsMCP.Core.Models;
using PackedTables.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Repositories {
  public interface ISettingsRepository {
    public string SettingsFilePathName { get; set; }
    public PackedTableSet SettingsTableSet { get; }
    public TableModel SettingsTbl { get; set; }
    public SecuritySettings GetSecuritySettings();

    public VersionSettings GetVersionSettings();

    public event Action OnSettingsLoadedEvent;
    public void Save();
    public void Load();

    public List<string> AllowedExtensions {get; set; }
    public List<string> BlockedExtensions { get; set; }
    public List<string> BlockedFolders {get; set; }
    public List<string> BlockedFileNames { get; set; }
    public long MaxFileSizeBytes { get; }
    public string MaxFileSize { get; set;}
    public long MaxFileWriteSizeBytes {get; }
    public string MaxFileWriteSize { get; set; }
    public bool AllowFileWrites {get; set; }

  }
}
