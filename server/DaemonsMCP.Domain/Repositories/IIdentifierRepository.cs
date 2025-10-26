using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DaemonsMCP.Domain.Entities;

namespace DaemonsMCP.Domain.Repositories;

public interface IIdentifierRepository {
  Task<Identifier?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
  Task<Identifier> GetOrCreateAsync(string name, CancellationToken cancellationToken = default);
  Task<List<Identifier>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
  Task SaveChangesAsync(CancellationToken cancellationToken = default);

}