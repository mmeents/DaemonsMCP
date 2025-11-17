using DaemonsMCP.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaemonsMCP.Infrastructure.Persistence;

namespace DaemonsMCP.Infrastructure.Services {
  public class DatabaseManagementService : IDatabaseManagementService {
    private readonly DaemonsMcpDbContext _context;

    public DatabaseManagementService(DaemonsMcpDbContext context) {
      _context = context;
    }

    public async Task ResetDatabaseAsync(CancellationToken cancellationToken = default) {
      // Delete all records from the tables and reset identity columns
      // to be used for manual execution in development/testing environments.
      // Api and tool both index on startup this can be done if followed by a restart. 
      await _context.Database.ExecuteSqlRawAsync(@"
            DELETE FROM dbo.ObjectHierarchies;
            DELETE FROM dbo.FileSystemNodes;
            DELETE FROM dbo.Identifiers;
            DBCC CHECKIDENT ('ObjectHierarchies', RESEED, 0);
            DBCC CHECKIDENT ('FileSystemNodes', RESEED, 0);
            DBCC CHECKIDENT ('Identifiers', RESEED, 0);
        ", cancellationToken);
    }
  }

}
