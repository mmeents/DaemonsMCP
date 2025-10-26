using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DaemonsMCP.Domain.Entities;

namespace DaemonsMCP.Infrastructure.Persistence.Configurations;

public class IndexQueueConfiguration : IEntityTypeConfiguration<IndexQueue> {
  public void Configure(EntityTypeBuilder<IndexQueue> builder) {
    builder.HasKey(q => q.Id);

    // Properties
    builder.Property(q => q.FilePath)
        .IsRequired()
        .HasMaxLength(2000);

    builder.Property(q => q.Status)
        .IsRequired()
        .HasConversion<int>();

    builder.Property(q => q.ErrorMessage)
        .HasMaxLength(2000);

    // Relationships
    builder.HasOne(q => q.Project)
        .WithMany()
        .HasForeignKey(q => q.ProjectId)
        .OnDelete(DeleteBehavior.NoAction);

    builder.HasOne(q => q.FileSystemNode)
        .WithMany()
        .HasForeignKey(q => q.FileSystemNodeId)
        .OnDelete(DeleteBehavior.Cascade);

    // Indexes - CRITICAL for FIFO queue performance
    builder.HasIndex(q => q.Status);
    builder.HasIndex(q => new { q.Status, q.CreatedAt });  // For "get next pending"
    builder.HasIndex(q => q.ProjectId);
  }
}
