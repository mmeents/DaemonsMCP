using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DaemonsMCP.Domain.Entities;

namespace DaemonsMCP.Infrastructure.Persistence.Configurations;

public class ItemTypeConfiguration : IEntityTypeConfiguration<ItemType> {
  public void Configure(EntityTypeBuilder<ItemType> builder) {
    builder.HasKey(it => it.Id);

    builder.Property(it => it.Name)
        .IsRequired()
        .HasMaxLength(200);

    builder.Property(it => it.Description)
        .IsRequired()
        .HasMaxLength(2000);

    builder.Property(it => it.Rank)
        .IsRequired();

    // Self-referencing relationship for hierarchy
    builder.HasOne(it => it.Parent)
        .WithMany(it => it.Children)
        .HasForeignKey(it => it.ParentId)
        .OnDelete(DeleteBehavior.Restrict);

    // Indexes
    builder.HasIndex(it => it.ParentId);
    builder.HasIndex(it => it.Name);
    builder.HasIndex(it => new { it.ParentId, it.Rank });

    // Seed data - Root hierarchy
    builder.HasData(
        // System root types
        new { Id = 1, Name = "None", Description = "Reserved null type - default unset value", Rank = 0, ParentId = (int?)null },
        new { Id = 2, Name = "Categories", Description = "Internal root - high-level grouping for type categories", Rank = 0, ParentId = (int?)null },
        
        // Category parents
        new { Id = 3, Name = "ItemTypes", Description = "Parent container for all item type definitions", Rank = 1, ParentId = (int?)2 },
        new { Id = 4, Name = "StatusTypes", Description = "Parent container for all status type definitions", Rank = 2, ParentId = (int?)2 },
        
        // Default Item Types
        new { Id = 5, Name = "Todo", Description = "A task or action item to be completed", Rank = 1, ParentId = (int?)3 },
        new { Id = 6, Name = "Readme", Description = "Documentation or informational content", Rank = 2, ParentId = (int?)3 },
        new { Id = 7, Name = "Note", Description = "General note or observation", Rank = 3, ParentId = (int?)3 },
        
        // Default Status Types
        new { Id = 10, Name = "Not Started", Description = "Item has not been started yet", Rank = 1, ParentId = (int?)4 },
        new { Id = 11, Name = "In Progress", Description = "Item is currently being worked on", Rank = 2, ParentId = (int?)4 },
        new { Id = 12, Name = "Complete", Description = "Item is finished", Rank = 3, ParentId = (int?)4 },
        new { Id = 13, Name = "On Hold", Description = "Item is paused or waiting", Rank = 4, ParentId = (int?)4 },
        new { Id = 14, Name = "Cancelled", Description = "Item was cancelled and will not be completed", Rank = 5, ParentId = (int?)4 }
    );
  }
}
