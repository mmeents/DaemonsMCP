using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DaemonsMCP.Domain.Entities;

namespace DaemonsMCP.Infrastructure.Persistence.Configurations;

public class FileSystemNodeConfiguration : IEntityTypeConfiguration<FileSystemNode> {
  public void Configure(EntityTypeBuilder<FileSystemNode> builder) {
    builder.HasKey(f => f.Id);

    builder.Property(f => f.Name)
        .IsRequired()
        .HasMaxLength(255);

    builder.Property(f => f.RelativePath)
        .IsRequired()
        .HasMaxLength(2000);

    builder.Property(f => f.Extension)
        .HasMaxLength(50);

    builder.Property(f => f.IsDirectory)
        .IsRequired();

    // Relationship to Project
    builder.HasOne(f => f.Project)
        .WithMany()
        .HasForeignKey(f => f.ProjectId)
        .OnDelete(DeleteBehavior.Cascade);

    // Self-referencing relationship for hierarchy
    builder.HasOne(f => f.Parent)
        .WithMany(f => f.Children)
        .HasForeignKey(f => f.ParentId)
        .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete issues

    // Indexes
    builder.HasIndex(f => f.ProjectId);
    builder.HasIndex(f => f.ParentId);
    builder.HasIndex(f => new { f.ProjectId, f.ParentId });
    builder.HasIndex(f => new { f.ProjectId, f.RelativePath }).IsUnique();
  }
}