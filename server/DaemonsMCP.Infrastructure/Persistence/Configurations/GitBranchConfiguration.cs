using DaemonsMCP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Infrastructure.Persistence.Configurations {
  public class GitBranchConfiguration : IEntityTypeConfiguration<GitBranch> {
    public void Configure(EntityTypeBuilder<GitBranch> builder) {
      builder.ToTable("GitBranches");

      builder.HasKey(e => e.Id);

      // Properties
      builder.Property(e => e.BranchName)
          .IsRequired()
          .HasMaxLength(200);

      builder.Property(e => e.FullName)
          .IsRequired()
          .HasMaxLength(500);

      builder.Property(e => e.IsRemote)
          .IsRequired()
          .HasDefaultValue(false);

      builder.Property(e => e.IsCurrentBranch)
          .IsRequired()
          .HasDefaultValue(false);

      builder.Property(e => e.IsTracking)
          .IsRequired()
          .HasDefaultValue(false);

      builder.Property(e => e.TrackingBranchName)
          .HasMaxLength(500);

      builder.Property(e => e.LastCommitSha)
          .HasMaxLength(40);

      builder.Property(e => e.LastCommitMessage)
          .HasMaxLength(2000);

      builder.Property(e => e.LastCommitAuthor)
          .HasMaxLength(200);

      builder.Property(e => e.CreatedAt)
          .IsRequired()
          .HasDefaultValueSql("GETUTCDATE()");

      builder.Property(e => e.UpdatedAt)
          .IsRequired()
          .HasDefaultValueSql("GETUTCDATE()");

      // Relationships
      builder.HasOne(e => e.GitRepository)
          .WithMany(r => r.Branches)
          .HasForeignKey(e => e.GitRepositoryId)
          .OnDelete(DeleteBehavior.Cascade);

      // Indexes
      builder.HasIndex(e => e.GitRepositoryId)
          .HasDatabaseName("IX_GitBranches_GitRepositoryId");

      // Unique constraint - one branch name per repo
      builder.HasIndex(e => new { e.GitRepositoryId, e.FullName })
          .IsUnique()
          .HasDatabaseName("UQ_GitBranches_UniqueName");

      // Filtered index for fast lookup of current branch
      builder.HasIndex(e => new { e.GitRepositoryId, e.IsCurrentBranch })
          .HasFilter("[IsCurrentBranch] = 1")
          .HasDatabaseName("IX_GitBranches_IsCurrentBranch");
    }
  }
}
