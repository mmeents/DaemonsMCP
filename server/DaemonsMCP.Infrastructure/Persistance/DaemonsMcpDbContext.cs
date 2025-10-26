using Microsoft.EntityFrameworkCore;
using DaemonsMCP.Domain.Entities;

namespace DaemonsMCP.Infrastructure.Persistence;

public class DaemonsMcpDbContext : DbContext {
  public DaemonsMcpDbContext(DbContextOptions<DaemonsMcpDbContext> options) : base(options) {
  }

  public DbSet<Project> Projects => Set<Project>();
  public DbSet<FileSystemNode> FileSystemNodes => Set<FileSystemNode>();
  public DbSet<Setting> Settings => Set<Setting>();

  public DbSet<ObjectHierarchy> ObjectHierarchies => Set<ObjectHierarchy>();
  public DbSet<Identifier> Identifiers => Set<Identifier>();
  public DbSet<IdentifierType> IdentifierTypes => Set<IdentifierType>();

  public DbSet<IndexQueue> IndexQueues => Set<IndexQueue>();

  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    base.OnModelCreating(modelBuilder);

    // Apply configurations
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(DaemonsMcpDbContext).Assembly);
  }
}
