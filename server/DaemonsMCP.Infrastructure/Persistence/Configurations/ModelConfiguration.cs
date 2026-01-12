using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DaemonsMCP.Domain.Entities;

namespace DaemonsMCP.Infrastructure.Persistence.Configurations {
  public class ModelConfiguration : IEntityTypeConfiguration<Model> {
    public void Configure(EntityTypeBuilder<Model> builder) {
      builder.HasKey(m => m.Id);

      builder.Property(m => m.Name)
          .IsRequired()
          .HasMaxLength(200);

      builder.Property(m => m.Rank)
          .IsRequired();

      builder.Property(m => m.Code)
          .HasMaxLength(-1)  // NVARCHAR(MAX)
          .IsRequired(false);

      builder.Property(m => m.CreatedDate)
          .IsRequired()
          .HasDefaultValueSql("GETUTCDATE()");

      builder.Property(m => m.ModifiedDate)
          .IsRequired()
          .HasDefaultValueSql("GETUTCDATE()"); 

      // Relationship to Project
      builder.HasOne(m => m.Project)
          .WithMany()
          .HasForeignKey(m => m.ProjectId)
          .OnDelete(DeleteBehavior.Cascade);

      // Self-referencing relationship for hierarchy
      builder.HasOne(m => m.Parent)
          .WithMany(m => m.Children)
          .HasForeignKey(m => m.ParentId)
          .OnDelete(DeleteBehavior.Restrict);

      // Relationship to ModelType
      builder.HasOne(m => m.ModelType)
          .WithMany()
          .HasForeignKey(m => m.ModelTypeId)
          .OnDelete(DeleteBehavior.Restrict);

      // Relationship to Properties
      builder.HasMany(m => m.Properties)
          .WithOne(p => p.Model)
          .HasForeignKey(p => p.ModelId)
          .OnDelete(DeleteBehavior.Cascade);

      // Indexes
      builder.HasIndex(m => m.ProjectId);
      builder.HasIndex(m => m.ParentId);
      builder.HasIndex(m => m.ModelTypeId);
      builder.HasIndex(m => new { m.ProjectId, m.ParentId, m.Rank });
      builder.HasIndex(m => m.CreatedDate);
    }
  }
}
