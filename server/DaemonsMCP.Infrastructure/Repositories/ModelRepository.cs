using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Enums;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DaemonsMCP.Infrastructure.Repositories {
  public class ModelRepository(DaemonsMcpDbContext context) : IModelRepository {
    private readonly DaemonsMcpDbContext _context = context;
    public async Task<Model?> GetByIdAsync(int id, CancellationToken cancellationToken = default) {
      var query = _context.Models.AsQueryable();
      query = query.Include(model => model.Children);
      return await query.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<Model?> GetByIdWithPropertiesAsync(int id, CancellationToken cancellationToken = default) {
      return await _context.Models
          .Include(m => m.Properties)
            .ThenInclude(p => p.PropertyValueType)
          .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<Model?> GetByIdWithChildrenAsync(int id, int maxDepth = 1, CancellationToken cancellationToken = default) {
      // Load root model with properties
      var model = await _context.Models
        .Include(m => m.Properties)
          .ThenInclude(p => p.PropertyValueType)
        .Include(m => m.ModelType)
        .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

      if (model == null) return null;

      // Recursively load children
      await LoadChildrenRecursiveAsync(model, maxDepth, cancellationToken);

      return model;
    }

    private async Task LoadChildrenRecursiveAsync(Model parent, int remainingDepth, CancellationToken cancellationToken) {
      if (remainingDepth <= 0) return;

      // Load children with properties for this level
      await _context.Entry(parent)
        .Collection(m => m.Children)
        .Query()
        .Include(c => c.Properties)
          .ThenInclude(p => p.PropertyValueType)
        .Include(c => c.ModelType)
        .LoadAsync(cancellationToken);

      // Recursively load grandchildren
      if (remainingDepth > 1 && parent.Children?.Any() == true) {
        foreach (var child in parent.Children) {
          await LoadChildrenRecursiveAsync(child, remainingDepth - 1, cancellationToken);
        }
      }
    }

    public async Task<List<Model>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default) {
      return await _context.Models
          .Where(m => m.ProjectId == projectId)
          .ToListAsync(cancellationToken);
    }

    public async Task<List<Model>> SearchAsync(int projectId, int? parentId = null, int? modelTypeId = null,
                                               string? nameFilter = null, CancellationToken cancellationToken = default) {
      var query = _context.Models.AsQueryable();
      query = query.Where(m => m.ProjectId == projectId);
      query = query.Where(m => m.ParentId == parentId);
      if (modelTypeId.HasValue) {
        query = query.Where(m => m.ModelTypeId == modelTypeId.Value);
      }
      if (!string.IsNullOrEmpty(nameFilter)) {
        query = query.Where(m => EF.Functions.Like(m.Name, $"%{nameFilter}%"));
      }
      return await query.ToListAsync(cancellationToken);
    }

    public async Task<int> AddAsync(Model model, CancellationToken cancellationToken = default) {
      _context.Models.Add(model);
      await _context.SaveChangesAsync(cancellationToken);
      await SyncDefaultsByModelIdAsync(model.Id, cancellationToken);
      return model.Id;
    }

    public async Task UpdateAsync(Model model, CancellationToken cancellationToken = default) {
      _context.Models.Update(model);
      await _context.SaveChangesAsync(cancellationToken);
      await SyncDefaultsByModelIdAsync(model.Id, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default) {
      var model = await _context.Models.FindAsync(new object[] { id }, cancellationToken);
      if (model != null) {
        _context.Models.Remove(model);
        await _context.SaveChangesAsync(cancellationToken);
      }
    }

    public async Task<List<Model>> GetByParentIdAsync(int? parentId, CancellationToken cancellationToken = default) {
      return await _context.Models
          .Include(i => i.ModelType)          
          .Where(i => i.ParentId == parentId)
          .OrderBy(i => i.Rank)
          .ThenBy(i => i.Name)
          .ToListAsync(cancellationToken);
    }

    public async Task<bool> SyncDefaultsByModelIdAsync(int modelId, CancellationToken cancellationToken = default) {
      var model = await _context.Models.FindAsync(new object[] { modelId }, cancellationToken);
      var properties = await _context.ModelProperties
          .Where(p => p.ModelId == modelId)
          .ToListAsync(cancellationToken);
      bool updated = false;
      var expected = GetDefaultPropertiesByModelId(model?.ModelTypeId ?? 0);

      foreach (var property in expected) {
        var existing = properties.FirstOrDefault(p => p.PropertyKey == property.PropertyKey);

        if (existing == null) {
          existing = new ModelProperty {
            ModelId = modelId,
            PropertyKey = property.PropertyKey,
            PropertyValue = property.PropertyValue,
            PropertyValueTypeId = property.PropertyValueTypeId
          };
          _context.ModelProperties.Add(existing);
          updated = true;
        }
      }
      if (updated) {
        await _context.SaveChangesAsync(cancellationToken);
      }
      return updated;
    }

    private IList<ModelPropertyDto> GetDefaultPropertiesByModelId(int modelTypeId) {

      var returnList = new List<ModelPropertyDto>();

      ModelPropExt.GetDefaultPropertiesByModelId(modelTypeId)
        .ForEach(prop => {
          returnList.Add(prop);
        });

      return returnList;
    }

  }
}
