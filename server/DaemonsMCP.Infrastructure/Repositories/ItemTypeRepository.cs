using Microsoft.EntityFrameworkCore;
using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Infrastructure.Persistence;

namespace DaemonsMCP.Infrastructure.Repositories;

public class ItemTypeRepository : IItemTypeRepository {
  private readonly DaemonsMcpDbContext _context;

  public ItemTypeRepository(DaemonsMcpDbContext context) {
    _context = context;
  }

  public async Task<ItemType?> GetByIdAsync(int id, CancellationToken cancellationToken = default) {
    return await _context.ItemTypes
        .FirstOrDefaultAsync(it => it.Id == id, cancellationToken);
  }

  public async Task<ItemType?> GetByIdWithChildrenAsync(int id, int maxDepth = 1, CancellationToken cancellationToken = default) {
    var query = _context.ItemTypes.AsQueryable();

    // Recursively include children based on maxDepth
    for (int i = 0; i < maxDepth; i++) {
      query = query.Include(itemType => itemType.Children);
    }

    return await query.FirstOrDefaultAsync(it => it.Id == id, cancellationToken);
  }

  public async Task<ItemType?> GetByNameAsync(string name, CancellationToken cancellationToken = default) {
    return await _context.ItemTypes
        .FirstOrDefaultAsync(it => it.Name == name, cancellationToken);
  }

  public async Task<List<ItemType>> GetAllAsync(CancellationToken cancellationToken = default) {
    return await _context.ItemTypes
        .OrderBy(it => it.Rank)
        .ThenBy(it => it.Name)
        .ToListAsync(cancellationToken);
  }

  public async Task<List<ItemType>> GetByParentIdAsync(int? parentId, CancellationToken cancellationToken = default) {
    return await _context.ItemTypes
        .Where(it => it.ParentId == parentId)
        .OrderBy(it => it.Rank)
        .ThenBy(it => it.Name)
        .ToListAsync(cancellationToken);
  }

  public async Task<List<ItemType>> GetRootTypesAsync(CancellationToken cancellationToken = default) {
    return await GetByParentIdAsync(null, cancellationToken);
  }

  public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default) {
    return await _context.ItemTypes
        .AnyAsync(it => it.Id == id, cancellationToken);
  }

  public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default) {
    return await _context.ItemTypes
        .AnyAsync(it => it.Name == name, cancellationToken);
  }

  public async Task<bool> HasChildrenAsync(int id, CancellationToken cancellationToken = default) {
    return await _context.ItemTypes
        .AnyAsync(it => it.ParentId == id, cancellationToken);
  }

  public async Task<bool> IsInUseAsync(int id, CancellationToken cancellationToken = default) {
    // Check if any Items use this ItemType (either as ItemType or StatusType)
    return await _context.Items
        .AnyAsync(i => i.ItemTypeId == id || i.StatusTypeId == id, cancellationToken);
  }

  public async Task<ItemType> AddAsync(ItemType itemType, CancellationToken cancellationToken = default) {
    await _context.ItemTypes.AddAsync(itemType, cancellationToken);
    return itemType;
  }

  public Task UpdateAsync(ItemType itemType, CancellationToken cancellationToken = default) {
    _context.ItemTypes.Update(itemType);
    return Task.CompletedTask;
  }

  public Task DeleteAsync(ItemType itemType, CancellationToken cancellationToken = default) {
    _context.ItemTypes.Remove(itemType);
    return Task.CompletedTask;
  }

  public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
    return await _context.SaveChangesAsync(cancellationToken);
  }
}
