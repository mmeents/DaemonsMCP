using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Repositories {
  public interface IDatabaseManagementService {
    Task ResetDatabaseAsync(CancellationToken cancellationToken = default);
  }
}
