using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using DaemonsMCP.Infrastructure.Persistence;

namespace DaemonsMCP.Infrastructure.Repositories {
  public class ModelPropertyRepository(DaemonsMcpDbContext context) : IModelPropertyRepository {
    private readonly DaemonsMcpDbContext _context = context;
    public async Task<ModelProperty?> GetByIdAsync(int id, CancellationToken cancellationToken = default) {
      return await _context.ModelProperties.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<List<ModelProperty>> GetByModelIdAsync(int modelId, CancellationToken cancellationToken = default) {
      return await _context.ModelProperties
          .Where(p => p.ModelId == modelId)
          .ToListAsync(cancellationToken);
    }

    public async Task<ModelProperty?> GetByModelAndKeyAsync(int modelId, string propertyKey, CancellationToken cancellationToken = default) {
      return await _context.ModelProperties
          .FirstOrDefaultAsync(p => p.ModelId == modelId && p.PropertyKey == propertyKey, cancellationToken);
    }

    public async Task<int> AddAsync(ModelProperty property, CancellationToken cancellationToken = default) {
      _context.ModelProperties.Add(property);
      await _context.SaveChangesAsync(cancellationToken);      
      return property.Id;
    }

    public async Task UpdateAsync(ModelProperty property, CancellationToken cancellationToken = default) {
      _context.ModelProperties.Update(property);
      await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default) {
      var property = await _context.ModelProperties.FindAsync(new object[] { id }, cancellationToken);
      if (property != null) {
        _context.ModelProperties.Remove(property);
        await _context.SaveChangesAsync(cancellationToken);
      }
    }

  }
}
