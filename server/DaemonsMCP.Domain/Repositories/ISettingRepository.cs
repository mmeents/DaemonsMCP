using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DaemonsMCP.Domain.Entities;

namespace DaemonsMCP.Domain.Repositories;

public interface ISettingRepository {
  // Queries
  Task<Setting?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
  Task<Setting?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
  Task<List<Setting>> GetAllAsync(CancellationToken cancellationToken = default);
  Task<Dictionary<string, string>> GetAllAsDictionaryAsync(CancellationToken cancellationToken = default);
  Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

  // Commands
  Task<Setting> AddAsync(Setting setting, CancellationToken cancellationToken = default);
  Task UpdateAsync(Setting setting, CancellationToken cancellationToken = default);
  Task DeleteAsync(Setting setting, CancellationToken cancellationToken = default);

  // Unit of Work
  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}