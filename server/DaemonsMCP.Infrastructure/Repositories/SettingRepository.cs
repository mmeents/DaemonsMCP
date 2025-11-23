using DaemonsMCP.Domain.Constants;
using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Domain.Extensions;
using DaemonsMCP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Infrastructure.Repositories;

public class SettingRepository : ISettingRepository {
  private readonly DaemonsMcpDbContext _context;
  private readonly ILogger<SettingRepository> _logger;

  public SettingRepository(DaemonsMcpDbContext context, ILogger<SettingRepository> logger) {
    _context = context;
    _logger = logger;
  }

  public Setting? this[string key] {
    get {
      var setting = _context.Settings.FirstOrDefault(s => s.Key == key);      
      return setting;
    }
    set {
      var existingSetting = _context.Settings.FirstOrDefault(s => s.Key == key);
      if (existingSetting != null) {
        if (value == null) {
          _context.Settings.Remove(existingSetting);
          return;
        }
        existingSetting.UpdateValue(value.Value);
        _context.Settings.Update(existingSetting);
      } else {
        if (value == null) {
          return;
        }
        var newSetting = new Setting(key, value.Value);
        _context.Settings.Add(newSetting);
      }
    }
  }

  public async Task<Setting?> GetByIdAsync(int id, CancellationToken cancellationToken = default) {
    return await _context.Settings
        .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
  }

  public async Task<Setting?> GetByKeyAsync(string key, CancellationToken cancellationToken = default) {
    return await _context.Settings
        .FirstOrDefaultAsync(s => s.Key == key, cancellationToken);
  }

  public async Task<List<Setting>> GetAllAsync(CancellationToken cancellationToken = default) {
    return await _context.Settings
        .OrderBy(s => s.Key)
        .ToListAsync(cancellationToken);
  }

  public async Task<Dictionary<string, string>> GetAllAsDictionaryAsync(CancellationToken cancellationToken = default) {
    return await _context.Settings
        .ToDictionaryAsync(s => s.Key, s => s.Value, cancellationToken);
  }

  public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default) {
    return await _context.Settings
        .AnyAsync(s => s.Key == key, cancellationToken);
  }

  public async Task<Setting> AddAsync(Setting setting, CancellationToken cancellationToken = default) {
    await _context.Settings.AddAsync(setting, cancellationToken);
    return setting;
  }

  public Task UpdateAsync(Setting setting, CancellationToken cancellationToken = default) {
    _context.Settings.Update(setting);
    return Task.CompletedTask;
  }

  public Task DeleteAsync(Setting setting, CancellationToken cancellationToken = default) {
    _context.Settings.Remove(setting);
    return Task.CompletedTask;
  }

  public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
    return await _context.SaveChangesAsync(cancellationToken);
  }

  public long MaxFileSizeBytes {
    get {
      var setting = this[Cx.MaxFileSizeSetting];
      if (string.IsNullOrEmpty(setting?.Value)) return 50 * 1024;  // 50KB default
      try {
        var size = setting.Value.ParseFileSize();
        return size;
      } catch (Exception ex) {
        _logger.LogError(ex, $"Error parsing MaxFileSize setting: '{setting}'. Defaulting to 50KB.");
        return 50 * 1024;
      }
    }
  }

  public string MaxFileSize {
    get {
      var size = this[Cx.MaxFileSizeSetting];
      if (string.IsNullOrEmpty(size?.Value)) return "50KB";
      return size.Value;
    }
    set {
      if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));
      try {
        var size = value.ParseFileSize(); // Validate
      } catch (Exception ex) {
        throw new ArgumentException($"Invalid file size format: '{value}'. Expected formats like '100 KB', '1 MB', '2 GB'.", ex);
      }
      this[Cx.MaxFileSizeSetting] = new Setting(Cx.MaxFileSizeSetting, value);
    }
  }

  public long MaxFileWriteSizeBytes {
    get {
      var setting = this[Cx.MaxFileWriteSizeSetting];
      if (string.IsNullOrEmpty(setting?.Value)) return 500 * 1024;  // 0.5MB default
      try {
        var size = setting.Value.ParseFileSize();
        return size;
      } catch (Exception ex) {
        _logger.LogError(ex, $"Error parsing MaxFileWriteSize setting: '{setting}'. Defaulting to 500KB.");
        return 500 * 1024;
      }
    }
  }

  public string MaxFileWriteSize {
    get {
      var size = this[Cx.MaxFileWriteSizeSetting];
      if (string.IsNullOrEmpty(size?.Value)) return "500KB";
      return size.Value;
    }
    set {
      if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));
      try {
        var size = value.ParseFileSize(); // Validate
      } catch (Exception ex) {
        throw new ArgumentException($"Invalid file size format: '{value}'. Expected formats like '100 KB', '1 MB', '2 GB'.", ex);
      }
      this[Cx.MaxFileWriteSizeSetting] = new Setting(Cx.MaxFileWriteSizeSetting, value);
    }
  }



}