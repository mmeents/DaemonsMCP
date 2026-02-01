using DaemonsMCP.Domain.Entities;
namespace DaemonsMCP.Domain.Models;

public record ItemDto {
  public int Id { get; init; }
  public int? ParentId { get; init; }
  public int ItemTypeId { get; init; }
  public string ItemTypeName { get; init; } = string.Empty;
  public int StatusTypeId { get; init; }
  public string StatusTypeName { get; init; } = string.Empty;
  public int Rank { get; init; }
  public string Name { get; init; } = string.Empty;
  public string Details { get; init; } = string.Empty;
  public DateTime Created { get; init; }
  public DateTime Modified { get; init; }
  public DateTime? Completed { get; init; }
  public int? ReferenceFileSystemId { get; init; }
  public int? ReferenceObjectHierarchyId { get; init; }
  public List<ItemDto> Children { get; init; } = new();
}

public record ItemTypeDto {
  public int Id { get; init; }
  public int? ParentId { get; init; }
  public int Rank { get; init; }
  public string Name { get; init; } = string.Empty;
  public string Description { get; init; } = string.Empty;
  public List<ItemTypeDto> Children { get; init; } = new();
}

public record ReadmeItemDto { 
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Details { get; init; } = string.Empty;
    public DateTime Modified { get; init; }
    public List<ReadmeItemDto> Children { get; init; } = new();
   
}

public static class ItemDtoExt {
  public static ItemDto ToDto(this Item item) =>
    new ItemDto {
      Id = item.Id,
      ParentId = item.ParentId,
      ItemTypeId = item.ItemTypeId,
      ItemTypeName = item.ItemType?.Name ?? string.Empty,
      StatusTypeId = item.StatusTypeId,
      StatusTypeName = item.StatusType?.Name ?? string.Empty,
      Rank = item.Rank,
      Name = item.Name,
      Details = item.Details,
      Created = item.Created,
      Modified = item.Modified,
      Completed = item.Completed,
      ReferenceFileSystemId = item.ReferenceFileSystemId,
      ReferenceObjectHierarchyId = item.ReferenceObjectHierarchyId,
      Children = item.Children?.Select(c => c.ToDto()).ToList() ?? new()
    };
}

