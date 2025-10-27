using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Models;

namespace DaemonsMCP.Domain.Repositories;

public interface IObjectHierarchyRepository {
  Task<ObjectHierarchy?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
  Task<List<ObjectHierarchy>> GetByFileSystemNodeIdAsync(int fileSystemNodeId, CancellationToken cancellationToken = default);
  Task<List<ObjectHierarchy>> GetByParentIdAsync(int parentId, CancellationToken cancellationToken = default);
  Task<ObjectHierarchy> GetOrCreateAsync(ObjectHierarchy hierarchy, CancellationToken cancellationToken = default);
  Task DeleteByFileSystemNodeIdAsync(int fileSystemNodeId, CancellationToken cancellationToken = default);
  Task DeleteRangeAsync(IEnumerable<ObjectHierarchy> hierarchies, CancellationToken cancellationToken = default);
  Task SaveChangesAsync(CancellationToken cancellationToken = default);
  Task<SearchObjectHierarchyResult> Search(SearchObjectHierarchyQuery request, CancellationToken cancellationToken);
}

