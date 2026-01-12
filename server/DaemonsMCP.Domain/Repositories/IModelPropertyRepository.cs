using DaemonsMCP.Domain.Entities;

namespace DaemonsMCP.Domain.Repositories {
  public interface IModelPropertyRepository {
    Task<ModelProperty?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<ModelProperty>> GetByModelIdAsync(int modelId, CancellationToken cancellationToken = default);
    Task<ModelProperty?> GetByModelAndKeyAsync(int modelId, string propertyKey, CancellationToken cancellationToken = default);
    Task<int> AddAsync(ModelProperty property, CancellationToken cancellationToken = default);
    Task UpdateAsync(ModelProperty property, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
  }
}
