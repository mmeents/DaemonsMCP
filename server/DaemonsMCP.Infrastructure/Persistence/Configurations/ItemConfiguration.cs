using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DaemonsMCP.Domain.Entities;

namespace DaemonsMCP.Infrastructure.Persistence.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item> {
  public void Configure(EntityTypeBuilder<Item> builder) {
    builder.HasKey(i => i.Id);

    builder.Property(i => i.Name)
        .IsRequired()
        .HasMaxLength(500);

    builder.Property(i => i.Details)
        .IsRequired()
        .HasMaxLength(8000);

    builder.Property(i => i.Rank)
        .IsRequired();

    builder.Property(i => i.Created)
        .IsRequired();

    builder.Property(i => i.Modified)
        .IsRequired();

    builder.Property(i => i.Completed)
        .IsRequired(false);

    // Relationship to ItemType (for type)
    builder.HasOne(i => i.ItemType)
        .WithMany()
        .HasForeignKey(i => i.ItemTypeId)
        .OnDelete(DeleteBehavior.Restrict);

    // Relationship to ItemType (for status) - uses same table!
    builder.HasOne(i => i.StatusType)
        .WithMany()
        .HasForeignKey(i => i.StatusTypeId)
        .OnDelete(DeleteBehavior.Restrict);

    // Self-referencing relationship for hierarchy
    builder.HasOne(i => i.Parent)
        .WithMany(i => i.Children)
        .HasForeignKey(i => i.ParentId)
        .OnDelete(DeleteBehavior.Restrict);

    // Optional relationship to FileSystemNode - changed to Restrict to avoid cascade cycles
    builder.HasOne(i => i.ReferenceFileSystem)
        .WithMany()
        .HasForeignKey(i => i.ReferenceFileSystemId)
        .OnDelete(DeleteBehavior.Restrict)
        .IsRequired(false);

    // Optional relationship to ObjectHierarchy - changed to Restrict to avoid cascade cycles
    builder.HasOne(i => i.ReferenceObjectHierarchy)
        .WithMany()
        .HasForeignKey(i => i.ReferenceObjectHierarchyId)
        .OnDelete(DeleteBehavior.Restrict)
        .IsRequired(false);

    // Indexes
    builder.HasIndex(i => i.ParentId);
    builder.HasIndex(i => i.ItemTypeId);
    builder.HasIndex(i => i.StatusTypeId);
    builder.HasIndex(i => i.ReferenceFileSystemId);
    builder.HasIndex(i => i.ReferenceObjectHierarchyId);
    builder.HasIndex(i => new { i.ParentId, i.Rank });
    builder.HasIndex(i => new { i.ItemTypeId, i.StatusTypeId });
    builder.HasIndex(i => i.Created);
    builder.HasIndex(i => i.Completed);

    // Seed data - Root items for README and TODO systems
    var now = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    
    builder.HasData(
        // README Documentation Root
        new {
            Id = 1,
            ParentId = (int?)null,
            ItemTypeId = 6,  // Readme type
            StatusTypeId = 12,  // Complete status
            Rank = 1,
            Name = "Daemons3MCP Documentation",
            Details = "Living documentation for the Daemons3MCP tool. This documentation is stored as hierarchical nodes and can be extended through the nodes interface. " +
                     "The system provides tools for file management, code indexing, and hierarchical note/task organization.",
            Created = now,
            Modified = now,
            Completed = (DateTime?)null,
            ReferenceFileSystemId = (int?)null,
            ReferenceObjectHierarchyId = (int?)null
        },
        
        // TODO Root - where all todo lists attach
        new {
            Id = 2,
            ParentId = (int?)null,
            ItemTypeId = 5,  // Todo type
            StatusTypeId = 11,  // In Progress status
            Rank = 2,
            Name = "Todo Root",
            Details = "Root container for all todo lists. Todo lists are created as children of this node. Use make-todo-list to create new lists, " +
                     "and get-next-todo to retrieve the next actionable item.",
            Created = now,
            Modified = now,
            Completed = (DateTime?)null,
            ReferenceFileSystemId = (int?)null,
            ReferenceObjectHierarchyId = (int?)null
        }
    );
  }
}
