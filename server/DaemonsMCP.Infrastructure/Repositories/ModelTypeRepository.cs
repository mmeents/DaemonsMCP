using DaemonsMCP.Domain.Enums;
using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DaemonsMCP.Infrastructure.Repositories {
  public class ModelTypeRepository(DaemonsMcpDbContext context) : IModelTypeRepository {
    private readonly DaemonsMcpDbContext _context = context;

    public async Task<ModelType> AddAsync(ModelType modelType, CancellationToken cancellationToken = default) {
      _context.ModelTypes.Add(modelType);
      await _context.SaveChangesAsync(cancellationToken);
      return modelType;
    }

    public async Task DeleteAsync(ModelType modelType, CancellationToken cancellationToken = default) {
      _context.ModelTypes.Remove(modelType);
      await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default) {
      return await _context.ModelTypes.AnyAsync(mt => mt.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default) {
      return await _context.ModelTypes.AnyAsync(mt => mt.Name == name, cancellationToken);
    }

    public async Task<List<ModelType>> GetAllAsync(CancellationToken cancellationToken = default) {
      return await _context.ModelTypes.ToListAsync(cancellationToken);
    }

    public async Task<List<ModelType>> GetByCategoryTypeIdAsync(int? categoryTypeId, CancellationToken cancellationToken = default) {
      return await _context.ModelTypes
          .Where(mt => mt.CategoryTypeId == categoryTypeId)
          .ToListAsync(cancellationToken);
    }

    public async Task<List<ModelType>> GetByEditorTypeIdAsync(int? editorTypeId, CancellationToken cancellationToken = default) {
      return await _context.ModelTypes
          .Where(mt => mt.EditorTypeId == editorTypeId)
          .OrderBy(it => it.TypeRank)
          .ThenBy(it => it.Name)
          .ToListAsync(cancellationToken);
    }

    public async Task<ModelType?> GetByIdAsync(int id, CancellationToken cancellationToken = default) {
      return await _context.ModelTypes.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<ModelType?> GetByIdWithChildrenAsync(int id, int maxDepth = 1, CancellationToken cancellationToken = default) {
        var query = _context.ModelTypes.AsQueryable();
      // Recursively include children based on maxDepth
      for( int i = 0; i < maxDepth; i++) {
        query = query.Include(modelType => modelType.Children);
      }
      return await query.FirstOrDefaultAsync(mt => mt.Id == id, cancellationToken);
    }

    public async Task<ModelType?> GetByNameAsync(string name, CancellationToken cancellationToken = default) {
      return await _context.ModelTypes
          .FirstOrDefaultAsync(mt => mt.Name == name, cancellationToken);
    }

    public async Task<List<ModelType>> GetByOwnerTypeIdAsync(int? ownerTypeId, CancellationToken cancellationToken = default) {
      return await _context.ModelTypes
        .Where(mt => mt.OwnerTypeId == ownerTypeId)
        .OrderBy(it => it.TypeRank)
        .ThenBy(it => it.Name)
        .ToListAsync(cancellationToken);
    }

    public async Task<List<ModelType>> GetSqlDataTypesAsync(CancellationToken cancellationToken = default) {
      return await _context.ModelTypes
        .Where(mt => mt.OwnerTypeId == (int)Mte.SqlTypes)
        .OrderBy(it => it.TypeRank)
        .ThenBy(it => it.Name)
        .ToListAsync(cancellationToken);
    }

    public async Task<List<ModelType>> GetEditorTypesAsync(CancellationToken cancellationToken = default) {
      return await _context.ModelTypes
          .Where(mt => mt.OwnerTypeId == (int)Mte.EditorList)
          .OrderBy(it => it.TypeRank)
          .ThenBy(it => it.Name)
          .ToListAsync(cancellationToken);
    }

    public async Task<List<ModelType>> GetProjectTemplatesAsync(CancellationToken cancellationToken = default) {
      return await _context.ModelTypes
        .Where(mt => mt.Id >= (int)Mte.ProjectModel && mt.Id <= 500)
        .OrderBy(it => it.TypeRank)
        .ThenBy(it => it.Name)
        .ToListAsync(cancellationToken);
    }

    public async Task<List<ModelType>> GetRootTypesAsync(CancellationToken cancellationToken = default) {
      return await _context.ModelTypes
          .Where(mt => mt.OwnerTypeId == null)
          .OrderBy(it => it.TypeRank)
          .ThenBy(it => it.Name)
          .ToListAsync(cancellationToken);
    }
            
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
      return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ModelType modelType, CancellationToken cancellationToken = default) {
      _context.ModelTypes.Update(modelType);
      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
