using DaemonsMCP.Domain.Entities;

namespace DaemonsMCP.Domain.Repositories;

public interface IItemTypeRepository {
  // Queries
  Task<ItemType?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
  Task<ItemType?> GetByIdWithChildrenAsync(int id, int maxDepth = 1, CancellationToken cancellationToken = default);
  Task<ItemType?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
  Task<List<ItemType>> GetAllAsync(CancellationToken cancellationToken = default);
  Task<List<ItemType>> GetByParentIdAsync(int? parentId, CancellationToken cancellationToken = default);
  Task<List<ItemType>> GetRootTypesAsync(CancellationToken cancellationToken = default);
  Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
  Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
  Task<bool> HasChildrenAsync(int id, CancellationToken cancellationToken = default);
  Task<bool> IsInUseAsync(int id, CancellationToken cancellationToken = default);

  // Commands
  Task<ItemType> AddAsync(ItemType itemType, CancellationToken cancellationToken = default);
  Task UpdateAsync(ItemType itemType, CancellationToken cancellationToken = default);
  Task DeleteAsync(ItemType itemType, CancellationToken cancellationToken = default);

  // Unit of Work
  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
