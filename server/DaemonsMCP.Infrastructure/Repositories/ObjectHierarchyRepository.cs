using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DaemonsMCP.Infrastructure.Repositories;

public class ObjectHierarchyRepository : IObjectHierarchyRepository {
  private readonly DaemonsMcpDbContext _dbContext;

  public ObjectHierarchyRepository(DaemonsMcpDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<ObjectHierarchy?> GetByIdAsync(int id, CancellationToken cancellationToken = default) {
    return await _dbContext.ObjectHierarchies
        .FirstOrDefaultAsync(oh => oh.Id == id, cancellationToken);
  }

  public async Task<List<ObjectHierarchy>> GetByFileSystemNodeIdAsync(int fileSystemNodeId, CancellationToken cancellationToken = default) {
    return await _dbContext.ObjectHierarchies
        .Where(oh => oh.FileSystemNodeId == fileSystemNodeId)
        .ToListAsync(cancellationToken);
  }

  public async Task<List<ObjectHierarchy>> GetByParentIdAsync(int parentId, CancellationToken cancellationToken = default) {
    return await _dbContext.ObjectHierarchies
        .Where(oh => oh.ParentId == parentId)
        .ToListAsync(cancellationToken);
  }

  public async Task<ObjectHierarchy> GetOrCreateAsync(ObjectHierarchy hierarchy, CancellationToken cancellationToken = default) {
    // Upsert: Try to find existing by unique composite key
    var existing = await _dbContext.ObjectHierarchies
        .FirstOrDefaultAsync(oh =>
            oh.FileSystemNodeId == hierarchy.FileSystemNodeId &&
            oh.IdentifierId == hierarchy.IdentifierId &&
            oh.IdentifierTypeId == hierarchy.IdentifierTypeId &&
            oh.ParentId == hierarchy.ParentId,
            cancellationToken);

    if (existing != null) {
      // Update line numbers if they changed
      existing.LineStart = hierarchy.LineStart;
      existing.LineEnd = hierarchy.LineEnd;
      _dbContext.ObjectHierarchies.Update(existing);      
      return existing;
    }

    // Create new
    _dbContext.ObjectHierarchies.Add(hierarchy);
    await _dbContext.SaveChangesAsync(cancellationToken);
    return hierarchy;
  }

  public async Task DeleteByFileSystemNodeIdAsync(int fileSystemNodeId, CancellationToken cancellationToken = default) {
    var hierarchies = await GetByFileSystemNodeIdAsync(fileSystemNodeId, cancellationToken);
    _dbContext.ObjectHierarchies.RemoveRange(hierarchies);
  }

  public async Task SaveChangesAsync(CancellationToken cancellationToken = default) {
    await _dbContext.SaveChangesAsync(cancellationToken);
  }
}