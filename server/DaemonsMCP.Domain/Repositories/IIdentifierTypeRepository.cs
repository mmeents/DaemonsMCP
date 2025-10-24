using DaemonsMCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Repositories {
  public interface IIdentifierTypeRepository {
    Task<List<IdentifierType>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IdentifierType?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

  }
}
