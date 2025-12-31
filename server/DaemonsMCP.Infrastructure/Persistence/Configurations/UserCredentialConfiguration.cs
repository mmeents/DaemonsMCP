using DaemonsMCP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DaemonsMCP.Infrastructure.Persistence.Configurations {
  public class UserCredentialConfiguration : IEntityTypeConfiguration<UserCredential> {
    public void Configure(EntityTypeBuilder<UserCredential> builder) {
      builder.ToTable("UserCredentials");

      builder.HasKey(uc => uc.Id);

      builder.Property(uc => uc.UserId)
          .IsRequired();

      builder.Property(uc => uc.Name)
          .IsRequired()
          .HasMaxLength(200);

      builder.Property(uc => uc.ProviderType)
          .IsRequired()
          .HasConversion<int>();

      builder.Property(uc => uc.CredentialType)
          .IsRequired()
          .HasConversion<int>();

      builder.Property(uc => uc.EncryptedUsername)
          .HasMaxLength(500);

      builder.Property(uc => uc.EncryptedSecret)
          .IsRequired()
          .HasMaxLength(2000);

      builder.Property(uc => uc.CreatedDate)
          .IsRequired()
          .HasDefaultValueSql("GETUTCDATE()");

      builder.Property(uc => uc.LastUsedDate);

      builder.Property(uc => uc.IsActive)
          .IsRequired()
          .HasDefaultValue(true);

      // Relationships
      builder.HasOne(uc => uc.User)
          .WithMany()
          .HasForeignKey(uc => uc.UserId)
          .OnDelete(DeleteBehavior.Cascade);

      // Indexes
      builder.HasIndex(uc => uc.UserId);
      builder.HasIndex(uc => new { uc.UserId, uc.IsActive });
    }
  }
}
