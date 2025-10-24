using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Infrastructure.Persistence;

namespace DaemonsMCP.Infrastructure.Repositories;

public class SettingRepository : ISettingRepository {
  private readonly DaemonsMcpDbContext _context;

  public SettingRepository(DaemonsMcpDbContext context) {
    _context = context;
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
}