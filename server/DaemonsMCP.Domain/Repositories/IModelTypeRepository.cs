using DaemonsMCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Repositories {
  public interface IModelTypeRepository {
    // Queries - Basic
    Task<ModelType?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ModelType?> GetByIdWithChildrenAsync(int id, int maxDepth = 1, CancellationToken cancellationToken = default);
    Task<ModelType?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<ModelType>> GetAllAsync(CancellationToken cancellationToken = default);

    // Queries - Hierarchical (ModelType has Owner/Category/Editor instead of simple Parent)
    Task<List<ModelType>> GetByOwnerTypeIdAsync(int? ownerTypeId, CancellationToken cancellationToken = default);
    Task<List<ModelType>> GetByCategoryTypeIdAsync(int? categoryTypeId, CancellationToken cancellationToken = default);
    Task<List<ModelType>> GetByEditorTypeIdAsync(int? editorTypeId, CancellationToken cancellationToken = default);
    Task<List<ModelType>> GetRootTypesAsync(CancellationToken cancellationToken = default);  // Where OwnerTypeId is null

    // Queries - Filtered    
    Task<List<ModelType>> GetEditorTypesAsync(CancellationToken cancellationToken = default);  // Types that are editors (Id 10-50 range)
    Task<List<ModelType>> GetSqlDataTypesAsync(CancellationToken cancellationToken = default);  // Data type lookups (Id 100-199)
    Task<List<ModelType>> GetProjectTemplatesAsync(CancellationToken cancellationToken = default);  // Project templates (Id 200+)

    // Queries - Existence checks
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);

    // Commands
    Task<ModelType> AddAsync(ModelType modelType, CancellationToken cancellationToken = default);
    Task UpdateAsync(ModelType modelType, CancellationToken cancellationToken = default);
    Task DeleteAsync(ModelType modelType, CancellationToken cancellationToken = default);

    // Unit of Work
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
  }
}
