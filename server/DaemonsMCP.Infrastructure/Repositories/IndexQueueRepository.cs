using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DaemonsMCP.Infrastructure.Repositories { 

  public class IndexQueueRepository : IIndexQueueRepository {
    private readonly DaemonsMcpDbContext _context;

    public IndexQueueRepository(DaemonsMcpDbContext context) {
      _context = context;
    }

    public async Task<IndexQueue?> GetByIdAsync(int id) {
      return await _context.IndexQueues.FirstOrDefaultAsync(q => q.Id == id);
    }

    // Get next batch of pending items for processing (FIFO)
    public async Task<IEnumerable<IndexQueue>> GetPendingAsync(int? projectId = null, int batchSize = 10) {
      var query = _context.IndexQueues
          .Where(q => q.Status == IndexQueueStatus.Pending);

      if (projectId.HasValue) {
        query = query.Where(q => q.ProjectId == projectId.Value);
      }

      return await query
          .OrderBy(q => q.CreatedAt)  // FIFO - oldest first
          .Take(batchSize)
          .ToListAsync();
    }

    public async Task<IEnumerable<IndexQueue>> GetByStatusAsync(IndexQueueStatus status) {
      return await _context.IndexQueues
          .Where(q => q.Status == status)
          .OrderBy(q => q.CreatedAt)
          .ToListAsync();
    }

    public async Task<IEnumerable<IndexQueue>> GetByProjectAsync(int projectId) {
      return await _context.IndexQueues
          .Where(q => q.ProjectId == projectId)
          .OrderByDescending(q => q.CreatedAt)
          .ToListAsync();
    }

    public async Task AddAsync(IndexQueue queueItem) {
      await _context.IndexQueues.AddAsync(queueItem);
      await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<IndexQueue> queueItems) {
      await _context.IndexQueues.AddRangeAsync(queueItems);
      await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(IndexQueue queueItem) {
      _context.IndexQueues.Update(queueItem);
      await _context.SaveChangesAsync();
    }

    // Clean up old completed items (e.g., older than 7 days)
    public async Task<int> DeleteCompletedAsync(int olderThanDaysAgo = 7) {
      var cutoffDate = DateTime.UtcNow.AddDays(-olderThanDaysAgo);

      var itemsToDelete = await _context.IndexQueues
          .Where(q => q.Status == IndexQueueStatus.Completed && q.CompletedAt < cutoffDate)
          .ToListAsync();

      _context.IndexQueues.RemoveRange(itemsToDelete);
      await _context.SaveChangesAsync();

      return itemsToDelete.Count;
    }
  }

}