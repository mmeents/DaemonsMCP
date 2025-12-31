using DaemonsMCP.Domain.Enums;
using DaemonsMCP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DaemonsMCP.Infrastructure.Persistence.Configurations {
  public class GitRepositoryConfiguration : IEntityTypeConfiguration<GitRepository> {
    public void Configure(EntityTypeBuilder<GitRepository> builder) {
      builder.ToTable("GitRepositories");

      builder.HasKey(pr => pr.Id);
      builder.Property(pr => pr.ProjectId).IsRequired();
      builder.Property(pr => pr.UserCredentialId);

      builder.Property(pr => pr.RemoteUrl)
        .IsRequired()
        .HasMaxLength(1000);

      builder.Property(pr => pr.LocalPath)
        .IsRequired()
        .HasMaxLength(500);

      builder.Property(pr => pr.RemoteName)
        .IsRequired()
        .HasMaxLength(100)
        .HasDefaultValue("origin");

      builder.Property(pr => pr.CurrentBranchName)        
        .HasMaxLength(200);

      builder.Property(pr => pr.IsDirty)
        .IsRequired()
        .HasDefaultValue(false);

      builder.Property(pr => pr.ModifiedFileCount);
      builder.Property(pr => pr.UntrackedFileCount);

      builder.Property(pr => pr.LastFetchedAt);
      builder.Property(pr => pr.LastSyncedAt);

      builder.Property(pr => pr.LastSyncStatus)
        .IsRequired()
        .HasConversion<int>()
        .HasDefaultValue(SyncStatus.Unknown);

      builder.Property(pr => pr.LastSyncError)
        .HasMaxLength(2000);

      builder.Property(pr => pr.CreatedAt)
        .IsRequired()
        .HasDefaultValueSql("GETUTCDATE()");

      builder.Property(pr => pr.UpdatedAt)
        .IsRequired()
        .HasDefaultValueSql("GETUTCDATE()");

      // Relationships
      builder.HasOne(pr => pr.Project)
        .WithMany()
        .HasForeignKey(pr => pr.ProjectId)
        .OnDelete(DeleteBehavior.Cascade);

      builder.HasOne(pr => pr.UserCredential)
        .WithMany(uc => uc.GitRepositories)
        .HasForeignKey(pr => pr.UserCredentialId)
        .OnDelete(DeleteBehavior.SetNull);

      // Indexes
      builder.HasIndex(pr => pr.ProjectId)
        .HasDatabaseName("IX_GitRepositories_ProjectId");

      builder.HasIndex(pr => pr.UserCredentialId)
        .HasDatabaseName("IX_GitRepositories_UserCredentialId");

      builder.HasIndex(pr => new { pr.ProjectId, pr.LocalPath })
        .IsUnique()
        .HasDatabaseName("UQ_GitRepositories_ProjectPath");

    }
  }
}
