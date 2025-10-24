using DaemonsMCP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Infrastructure.Persistance.Configurations {
  public class ObjectHierarchyConfiguration : IEntityTypeConfiguration<ObjectHierarchy>{ 
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ObjectHierarchy> builder) {
      builder.HasKey(oh => oh.Id);
      builder.Property(oh => oh.ParentId);
      builder.Property(oh => oh.ProjectId).IsRequired();
      builder.Property(oh => oh.FileSystemNodeId).IsRequired();
      builder.Property(oh => oh.IdentifierId).IsRequired();
      builder.Property(oh => oh.IdentifierTypeId).IsRequired();
      builder.Property(oh => oh.LineStart).IsRequired();
      builder.Property(oh => oh.LineEnd).IsRequired();
      builder.Property(oh => oh.IndexedAt).IsRequired();

      // Relationships
      builder.HasOne(oh => oh.Project)
          .WithMany()
          .HasForeignKey(oh => oh.ProjectId)
          .OnDelete(DeleteBehavior.NoAction);

      builder.HasOne(oh => oh.FileSystemNode)
          .WithMany()
          .HasForeignKey(oh => oh.FileSystemNodeId)
          .OnDelete(DeleteBehavior.Cascade);

      builder.HasOne(oh => oh.Parent)
          .WithMany(oh => oh.Children)
          .HasForeignKey(oh => oh.ParentId)
          .IsRequired(false)
          .OnDelete(DeleteBehavior.NoAction);

      builder.HasOne(oh => oh.Identifier)
          .WithMany()
          .HasForeignKey(oh => oh.IdentifierId)
          .OnDelete(DeleteBehavior.NoAction);

      builder.HasOne(oh => oh.IdentifierType)
          .WithMany()
          .HasForeignKey(oh => oh.IdentifierTypeId)
          .OnDelete(DeleteBehavior.NoAction);

      // Indexes
      builder.HasIndex(oh => oh.ProjectId);
      builder.HasIndex(oh => oh.FileSystemNodeId);
      builder.HasIndex(oh => oh.ParentId);
      builder.HasIndex(oh => new { oh.ProjectId, oh.ParentId });
      builder.HasIndex(oh => oh.IdentifierId);
      builder.HasIndex(oh => oh.IdentifierTypeId);
    }

  }
}
