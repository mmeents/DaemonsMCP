using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace DaemonsMCP.Infrastructure.Persistence;

public class DaemonsMcpDbContext(DbContextOptions<DaemonsMcpDbContext> options) : DbContext(options) {
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

  public DbSet<ModelType> ModelTypes => Set<ModelType>();
  public DbSet<Model> Models => Set<Model>();
  public DbSet<ModelProperty> ModelProperties => Set<ModelProperty>();


  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    base.OnModelCreating(modelBuilder);    
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(DaemonsMcpDbContext).Assembly);    

  }
}
