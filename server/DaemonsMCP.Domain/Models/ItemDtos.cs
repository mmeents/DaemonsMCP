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
