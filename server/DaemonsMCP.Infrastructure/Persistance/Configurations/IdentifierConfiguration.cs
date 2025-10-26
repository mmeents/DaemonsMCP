using DaemonsMCP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Infrastructure.Persistance.Configurations {
  public class IdentifierConfiguration : IEntityTypeConfiguration<Identifier> {
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Identifier> builder) {
      builder.HasKey(i => i.Id);
      builder.Property(i => i.Name)
          .IsRequired()
          .HasMaxLength(4000);
      builder.HasIndex(i => i.Name).IsUnique();
    }
  }
}
