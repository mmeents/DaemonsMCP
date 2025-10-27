using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaemonsMCP.Domain.Models;

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

  public Task DeleteRangeAsync(IEnumerable<ObjectHierarchy> hierarchies, CancellationToken cancellationToken = default) {
    _dbContext.ObjectHierarchies.RemoveRange(hierarchies);
    return Task.CompletedTask;
  }

  public async Task SaveChangesAsync(CancellationToken cancellationToken = default) {
    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public async Task<SearchObjectHierarchyResult> Search(
    SearchObjectHierarchyQuery request, CancellationToken cancellationToken) {

    // Build the query
    var query = _dbContext.ObjectHierarchies
        .Include(oh => oh.FileSystemNode)
        .Include(oh => oh.Identifier)
        .Include(oh => oh.IdentifierType)
        .Include(oh => oh.Parent)
            .ThenInclude(p => p.Identifier)
        .Include(oh => oh.Parent)
            .ThenInclude(p => p.IdentifierType)
        .Where(oh => oh.ProjectId == request.ProjectId);

    // Apply filters
    if (!string.IsNullOrEmpty(request.SearchTerm)) {
      query = query.Where(oh => oh.Identifier.Name.Contains(request.SearchTerm));
    }

    if (request.IdentifierTypeId.HasValue) {
      query = query.Where(oh => oh.IdentifierTypeId == request.IdentifierTypeId.Value);
    }

    if (request.FileSystemNodeId.HasValue) {
      query = query.Where(oh => oh.FileSystemNodeId == request.FileSystemNodeId.Value);
    }

    if (request.ParentId.HasValue) {
      query = query.Where(oh => oh.ParentId == request.ParentId.Value);
    }

    // Get total count before paging
    var totalCount = await query.CountAsync(cancellationToken);

    // Apply ordering and paging
    var results = await query
        .OrderByDescending(oh => oh.IndexedAt)
        .Skip((request.PageNo - 1) * request.PageSize)
        .Take(request.PageSize)
        .Select(oh => new ObjectHierarchyNodeDto {
          Id = oh.Id,
          IdentifierName = oh.Identifier.Name,
          IdentifierTypeName = oh.IdentifierType.Name,
          IdentifierTypeId = oh.IdentifierTypeId,
          ParentId = oh.ParentId,
          ParentName = oh.Parent != null ? oh.Parent.Identifier.Name : null,
          ParentTypeName = oh.Parent != null ? oh.Parent.IdentifierType.Name : null,
          FileSystemNodeId = oh.FileSystemNodeId,
          RelativePath = oh.FileSystemNode.RelativePath,
          FileName = oh.FileSystemNode.Name,
          LineStart = oh.LineStart,
          LineEnd = oh.LineEnd,
          IndexedAt = oh.IndexedAt,
          ProjectId = oh.ProjectId
        })
        .ToListAsync(cancellationToken);

    return new SearchObjectHierarchyResult {
      Data = results,
      TotalCount = totalCount,
      PageNo = request.PageNo,
      PageSize = request.PageSize,
      SearchTerm = request.SearchTerm
    };
  }

}