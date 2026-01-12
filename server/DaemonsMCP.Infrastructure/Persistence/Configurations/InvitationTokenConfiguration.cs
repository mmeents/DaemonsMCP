using DaemonsMCP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DaemonsMCP.Infrastructure.Persistence.Configurations {
  public class InvitationTokenConfiguration : IEntityTypeConfiguration<InvitationToken> {
    public void Configure(EntityTypeBuilder<InvitationToken> builder) {
      builder.ToTable("InvitationTokens");

      builder.HasKey(x => x.Id);

      builder.Property(x => x.Token)
          .IsRequired()
          .HasMaxLength(500);

      builder.Property(x => x.InvitedEmail)
          .HasMaxLength(500);

       builder.Property(x => x.CreatedByUserId)
          .IsRequired();

      builder.Property(x => x.CreatedAt)
          .IsRequired();

      builder.Property(x => x.ExpiresAt)
          .IsRequired();

      builder.Property(x => x.IsUsed)
          .IsRequired();

      builder.Property(x => x.UsedAt)
          .IsRequired(false);

      builder.Property(x => x.UsedByUserId)
          .IsRequired(false);

      builder.HasOne(x => x.CreatedBy)
          .WithMany()
          .HasForeignKey(x => x.CreatedByUserId)
          .OnDelete(DeleteBehavior.Restrict);

      builder.HasOne(x => x.UsedBy)
          .WithMany()
          .HasForeignKey(x => x.UsedByUserId)
          .OnDelete(DeleteBehavior.Restrict);

      builder.HasIndex(x => x.Token).IsUnique();
    }
  }
}
