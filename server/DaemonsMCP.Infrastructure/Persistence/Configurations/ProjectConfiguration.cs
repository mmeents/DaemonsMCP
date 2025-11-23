using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DaemonsMCP.Domain.Entities;

namespace DaemonsMCP.Infrastructure.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project> {
  public void Configure(EntityTypeBuilder<Project> builder) {
    builder.HasKey(p => p.Id);

    builder.Property(p => p.Name)
        .IsRequired()
        .HasMaxLength(200);

    builder.Property(p => p.Description)
        .IsRequired()
        .HasMaxLength(1000);

    builder.Property(p => p.RootPath)
        .IsRequired()
        .HasMaxLength(1000);

    builder.Property(p => p.CreatedAt)
        .IsRequired();
  }
}
