using DaemonsMCP.Domain.Entities;

namespace DaemonsMCP.Domain.Repositories;

public interface IItemRepository {
  // Queries
  Task<Item?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
  Task<Item?> GetByIdWithChildrenAsync(int id, int maxDepth = 1, CancellationToken cancellationToken = default);
  Task<Item?> GetByTodoNameAsync(string name, CancellationToken cancellationToken = default);
  Task<List<Item>> GetAllAsync(CancellationToken cancellationToken = default);
  Task<List<Item>> GetByParentIdAsync(int? parentId, CancellationToken cancellationToken = default);
  Task<List<Item>> GetRootItemsAsync(CancellationToken cancellationToken = default);
  Task<List<Item>> GetByTypeIdAsync(int typeId, CancellationToken cancellationToken = default);
  Task<List<Item>> GetByStatusIdAsync(int statusId, CancellationToken cancellationToken = default);
  Task<List<Item>> SearchAsync(
    string? nameContains = null,
    string? detailsContains = null,
    int? typeId = null,
    int? statusId = null,
    int? parentId = null,
    CancellationToken cancellationToken = default);
  Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
  Task<bool> HasChildrenAsync(int id, CancellationToken cancellationToken = default);

  // Commands
  Task<Item> AddAsync(Item item, CancellationToken cancellationToken = default);
  Task UpdateAsync(Item item, CancellationToken cancellationToken = default);
  Task DeleteAsync(Item item, CancellationToken cancellationToken = default);
  Task DeleteWithChildrenAsync(int id, CancellationToken cancellationToken = default);

  // Unit of Work
  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
