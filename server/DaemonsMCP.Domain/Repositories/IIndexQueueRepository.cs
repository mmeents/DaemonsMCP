using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DaemonsMCP.Domain.Entities;

namespace DaemonsMCP.Domain.Repositories;

public interface IIndexQueueRepository {
  Task<IndexQueue?> GetByIdAsync(int id);

  // Queue operations
  Task<IEnumerable<IndexQueue>> GetPendingAsync(int? projectId = null, int batchSize = 10);
  Task<IEnumerable<IndexQueue>> GetByStatusAsync(IndexQueueStatus status);
  Task<IEnumerable<IndexQueue>> GetByProjectAsync(int projectId);

  // Add to queue
  Task AddAsync(IndexQueue queueItem);
  Task AddRangeAsync(IEnumerable<IndexQueue> queueItems);

  // Process queue
  Task UpdateAsync(IndexQueue queueItem);

  // Cleanup
  Task<int> DeleteCompletedAsync(int olderThanDaysAgo = 7);
}
