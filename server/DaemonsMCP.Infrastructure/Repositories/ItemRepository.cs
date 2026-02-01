using DaemonsMCP.Domain.Constants;
using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DaemonsMCP.Infrastructure.Repositories;

public class ItemRepository : IItemRepository {
  private readonly DaemonsMcpDbContext _context;
  private readonly IItemTypeRepository _itemTypeRepository;

  public ItemRepository(DaemonsMcpDbContext context, IItemTypeRepository itemTypeRepository) {
    _context = context;
    _itemTypeRepository = itemTypeRepository; 
  }

  public async Task<Item?> GetByIdAsync(int id, CancellationToken cancellationToken = default) {
    return await _context.Items
        .Include(i => i.ItemType)
        .Include(i => i.StatusType)
        .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
  }

  public async Task<Item?> GetByIdWithChildrenAsync(int id, int maxDepth = 1, CancellationToken cancellationToken = default) {
    var query = _context.Items
        .Include(i => i.ItemType)
        .Include(i => i.StatusType)
        .AsQueryable();

    // Recursively include children based on maxDepth
    for (int i = 0; i < maxDepth; i++) {
      query = query.Include(item => item.Children);
    }

    return await query.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
  }

  public async Task<Item?> GetByTodoNameAsync(string name, CancellationToken cancellationToken = default) {
    return await _context.Items
        .Include(i => i.ItemType)
        .Include(i => i.StatusType)
        .Where(i => i.ItemTypeId == Cx.ItemTypeIdTodo)
        .Where(i => i.StatusTypeId == Cx.StatusTypeIdInProgress || i.StatusTypeId == Cx.StatusTypeIdNotStarted)
        .Include(item => item.Children)
        .FirstOrDefaultAsync(i => i.Name == name, cancellationToken);
  }

  public async Task<List<Item>> GetAllAsync(CancellationToken cancellationToken = default) {
    return await _context.Items
        .Include(i => i.ItemType)
        .Include(i => i.StatusType)
        .OrderBy(i => i.Rank)
        .ThenBy(i => i.Name)
        .ToListAsync(cancellationToken);
  }

  public async Task<List<Item>> GetByParentIdAsync(int? parentId, CancellationToken cancellationToken = default) {
    return await _context.Items
        .Include(i => i.ItemType)
        .Include(i => i.StatusType)
        .Where(i => i.ParentId == parentId)
        .OrderBy(i => i.Rank)
        .ThenBy(i => i.Name)
        .ToListAsync(cancellationToken);
  }

  public async Task<List<Item>> GetRootItemsAsync(CancellationToken cancellationToken = default) {
    return await GetByParentIdAsync(null, cancellationToken);
  }

  public async Task<List<Item>> GetByTypeIdAsync(int typeId, CancellationToken cancellationToken = default) {
    return await _context.Items
        .Include(i => i.ItemType)
        .Include(i => i.StatusType)
        .Where(i => i.ItemTypeId == typeId)
        .OrderBy(i => i.Rank)
        .ThenBy(i => i.Name)
        .ToListAsync(cancellationToken);
  }

  public async Task<List<Item>> GetByStatusIdAsync(int statusId, CancellationToken cancellationToken = default) {
    return await _context.Items
        .Include(i => i.ItemType)
        .Include(i => i.StatusType)
        .Where(i => i.StatusTypeId == statusId)
        .OrderBy(i => i.Rank)
        .ThenBy(i => i.Name)
        .ToListAsync(cancellationToken);
  }

  public async Task<List<Item>> SearchAsync(
      string? nameContains = null,
      string? detailsContains = null,
      int? typeId = null,
      int? statusId = null,
      int? parentId = null,
      CancellationToken cancellationToken = default) {
    
    var query = _context.Items
        .Include(i => i.ItemType)
        .Include(i => i.StatusType)
        .AsQueryable();

    if (!string.IsNullOrWhiteSpace(nameContains)) {
      query = query.Where(i => i.Name.Contains(nameContains));
    }

    if (!string.IsNullOrWhiteSpace(detailsContains)) {
      query = query.Where(i => i.Details.Contains(detailsContains));
    }

    if (typeId.HasValue) {
      query = query.Where(i => i.ItemTypeId == typeId.Value);
    }

    if (statusId.HasValue) {
      query = query.Where(i => i.StatusTypeId == statusId.Value);
    }

    if (parentId.HasValue) {
      query = query.Where(i => i.ParentId == parentId.Value);
    } else { 
      query = query.Where(i => i.ParentId == null);
    }

    return await query
        .OrderBy(i => i.Rank)
        .ThenBy(i => i.Name)
        .ToListAsync(cancellationToken);
  }

  public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default) {
    return await _context.Items
        .AnyAsync(i => i.Id == id, cancellationToken);
  }

  public async Task<bool> HasChildrenAsync(int id, CancellationToken cancellationToken = default) {
    return await _context.Items
        .AnyAsync(i => i.ParentId == id, cancellationToken);
  }

  public async Task<Item> AddAsync(Item item, CancellationToken cancellationToken = default) {
    await _context.Items.AddAsync(item, cancellationToken);
    return item;
  }

  public Task UpdateAsync(Item item, CancellationToken cancellationToken = default) {
    _context.Items.Update(item);
    return Task.CompletedTask;
  }

  public Task DeleteAsync(Item item, CancellationToken cancellationToken = default) {
    _context.Items.Remove(item);
    return Task.CompletedTask;
  }

  public async Task DeleteWithChildrenAsync(int id, CancellationToken cancellationToken = default) {
    var item = await _context.Items
        .Include(i => i.Children)
        .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    
    if (item != null) {
      // Recursively delete children
      await DeleteChildrenRecursiveAsync(item, cancellationToken);
      _context.Items.Remove(item);
    }
  }

  private async Task DeleteChildrenRecursiveAsync(Item parent, CancellationToken cancellationToken) {
    var children = await _context.Items
        .Where(i => i.ParentId == parent.Id)
        .ToListAsync(cancellationToken);

    foreach (var child in children) {
      await DeleteChildrenRecursiveAsync(child, cancellationToken);
      _context.Items.Remove(child);
    }
  }

  public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
    return await _context.SaveChangesAsync(cancellationToken);
  }


  public async Task<ItemDto> MapToDto(Item item, int maxDepth, CancellationToken cancellationToken) {
    var itemType = await _itemTypeRepository.GetByIdAsync(item.ItemTypeId, cancellationToken);
    var statusType = await _itemTypeRepository.GetByIdAsync(item.StatusTypeId, cancellationToken);

    var dto = new ItemDto {
      Id = item.Id,
      ParentId = item.ParentId,
      ItemTypeId = item.ItemTypeId,
      ItemTypeName = itemType?.Name ?? string.Empty,
      StatusTypeId = item.StatusTypeId,
      StatusTypeName = statusType?.Name ?? string.Empty,
      Rank = item.Rank,
      Name = item.Name,
      Details = item.Details,
      Created = item.Created,
      Modified = item.Modified,
      Completed = item.Completed,
      ReferenceFileSystemId = item.ReferenceFileSystemId,
      ReferenceObjectHierarchyId = item.ReferenceObjectHierarchyId,
      Children = new List<ItemDto>()
    };

    if (maxDepth > 0 && item.Children.Any()) {
      foreach (var child in item.Children) {
        var childDto = await MapToDto(child, maxDepth - 1, cancellationToken);
        dto.Children.Add(childDto);
      }
    }

    return dto;
  }
}
