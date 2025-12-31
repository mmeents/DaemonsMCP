using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

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

  public DbSet<Item> Items => Set<Item>();
  public DbSet<ItemType> ItemTypes => Set<ItemType>();

  public DbSet<AccessToken> AccessTokens => Set<AccessToken>();
  public DbSet<User> Users => Set<User>();
  public DbSet<InvitationToken> InvitationTokens => Set<InvitationToken>();
  public DbSet<UserCredential> UserCredentials => Set<UserCredential>();
  public DbSet<GitRepository> GitRepositories => Set<GitRepository>();
  public DbSet<GitBranch> GitBranches => Set<GitBranch>();


  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplyConfigurationsFromAssembly(typeof(DaemonsMcpDbContext).Assembly);    
  }
}
