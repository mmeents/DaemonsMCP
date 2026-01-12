using DaemonsMCP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Infrastructure.Persistence.Configurations {
  public class ModelPropertyConfiguration : IEntityTypeConfiguration<ModelProperty> {
    public void Configure(EntityTypeBuilder<ModelProperty> builder) {
      builder.HasKey(p => p.Id);

      builder.Property(p => p.PropertyKey)
          .IsRequired()
          .HasMaxLength(100);

      builder.Property(p => p.PropertyValue)
          .HasMaxLength(-1)  // NVARCHAR(MAX)
          .IsRequired(false);

      builder.Property(p => p.PropertyValueTypeId)
        .IsRequired(false);

      // Unique constraint on ModelId + PropertyKey
      builder.HasIndex(p => new { p.ModelId, p.PropertyKey })
          .IsUnique();

      // Relationship to Model (handled by ModelConfiguration)

      // Optional relationship to ModelType for property definition
      builder.HasOne(p => p.PropertyValueType)
          .WithMany()
          .HasForeignKey(p => p.PropertyValueTypeId)
          .OnDelete(DeleteBehavior.Restrict)
          .IsRequired(false);

      // Indexes
      builder.HasIndex(p => p.ModelId);
      builder.HasIndex(p => p.PropertyKey);
      builder.HasIndex(p => p.PropertyValueTypeId);
    }
  }
}
