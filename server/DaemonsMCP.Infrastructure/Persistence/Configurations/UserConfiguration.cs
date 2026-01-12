using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DaemonsMCP.Domain.Entities;

namespace DaemonsMCP.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User> {
  public void Configure(EntityTypeBuilder<User> builder) {
    builder.HasKey(u => u.Id);

    builder.Property(u => u.Email)
        .IsRequired()
        .HasMaxLength(255);

    builder.Property(u => u.DisplayName)
        .IsRequired()
        .HasMaxLength(255);

    builder.Property(u => u.PasswordHash)
        .HasMaxLength(255);

    builder.Property(u => u.GoogleId)
        .HasMaxLength(255);

    builder.Property(u => u.GitHubId)
        .HasMaxLength(255);

    builder.Property(u => u.IsActive)
        .IsRequired()
        .HasDefaultValue(true);

    builder.Property(u => u.CreatedAt)
        .IsRequired();

    // Indexes
    builder.HasIndex(u => u.Email).IsUnique();
    builder.HasIndex(u => u.GoogleId).IsUnique();
    builder.HasIndex(u => u.GitHubId).IsUnique();
    builder.HasIndex(u => u.IsActive);
  }
}
