using DaemonsMCP.Domain.Entities;

namespace DaemonsMCP.Domain.Repositories {
  public interface IModelRepository {
    Task<Model?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Model?> GetByIdWithPropertiesAsync(int id, CancellationToken cancellationToken = default);
    Task<Model?> GetByIdWithChildrenAsync(int id, int maxDepth = 1, CancellationToken cancellationToken = default);
    Task<List<Model>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
    Task<List<Model>> GetByParentIdAsync(int? parentId, CancellationToken cancellationToken = default);
    Task<List<Model>> SearchAsync(int projectId, int? parentId = null, int? modelTypeId = null,
                                   string? nameFilter = null, CancellationToken cancellationToken = default);
    Task<int> AddAsync(Model model, CancellationToken cancellationToken = default);
    Task UpdateAsync(Model model, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
  }
}
