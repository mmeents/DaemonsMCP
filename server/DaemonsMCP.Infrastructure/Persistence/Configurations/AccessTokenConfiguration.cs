using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DaemonsMCP.Domain.Entities;

namespace DaemonsMCP.Infrastructure.Persistence.Configurations;

public class AccessTokenConfiguration : IEntityTypeConfiguration<AccessToken> {
  public void Configure(EntityTypeBuilder<AccessToken> builder) {
    builder.HasKey(a => a.Id);

    builder.Property(a => a.Token)
        .IsRequired()
        .HasMaxLength(500);

    builder.Property(a => a.Created)
        .IsRequired();

    builder.Property(a => a.Expires)
        .IsRequired();

    builder.Property(a => a.Used)
        .IsRequired(false);

    builder.Property(a => a.UsedUrl)
        .IsRequired(false)
        .HasMaxLength(4000);

    builder.Property(a => a.UsedBy)
        .IsRequired(false)
        .HasMaxLength(200);

    builder.Property(a => a.IsRevokedChain)
      .IsRequired().HasDefaultValue(false);

    builder.Property(a => a.UsedUrlNextToken)
        .IsRequired(false)
        .HasMaxLength(500);

    builder.Property(a => a.IssuedTo)
        .IsRequired()
        .HasMaxLength(200)
        .HasDefaultValue("unknown");

    // Self-referencing relationship for tracking issuing key
    builder.HasOne(a => a.Parent)
        .WithMany(a => a.Children)
        .HasForeignKey(a => a.ParentId)
        .OnDelete(DeleteBehavior.Restrict);

    // Indexes
    builder.HasIndex(a => a.Token)
        .IsUnique();  // Tokens should be unique

    builder.HasIndex(a => a.ParentId);
    builder.HasIndex(a => a.Expires);
    builder.HasIndex(a => a.Used);
    builder.HasIndex(a => a.UsedUrl);
    builder.HasIndex(a => a.UsedBy);
    builder.HasIndex(a => new { a.Expires, a.UsedUrl });  // Composite for finding valid tokens
  }
}
