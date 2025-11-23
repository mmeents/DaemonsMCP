using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DaemonsMCP.Infrastructure.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DaemonsMcpDbContext> {
  public DaemonsMcpDbContext CreateDbContext(string[] args) {
    // Build configuration
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false)
        .Build();

    // Build DbContextOptions
    var optionsBuilder = new DbContextOptionsBuilder<DaemonsMcpDbContext>();
    var connectionString = configuration.GetConnectionString("DefaultConnection");

    optionsBuilder.UseSqlServer(connectionString);

    return new DaemonsMcpDbContext(optionsBuilder.Options);
  }
}
