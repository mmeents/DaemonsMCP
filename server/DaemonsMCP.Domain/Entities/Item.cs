namespace DaemonsMCP.Domain.Entities {
  public class Item {
    public int Id { get; private set; }
    public int? ParentId { get; private set; }
    public int ItemTypeId { get; private set; }
    public int StatusTypeId { get; private set; }
    public int Rank { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Details { get; private set; } = string.Empty;
    
    public DateTime Created { get; private set; }
    public DateTime Modified { get; private set; }
    public DateTime? Completed { get; private set; }

    // Optional references to other entities
    public int? ReferenceFileSystemId { get; private set; }
    public int? ReferenceObjectHierarchyId { get; private set; }

    // Navigation properties
    public Item? Parent { get; private set; }
    public ICollection<Item> Children { get; private set; } = new List<Item>();
    public ItemType ItemType { get; private set; } = null!;
    public ItemType StatusType { get; private set; } = null!;
    public FileSystemNode? ReferenceFileSystem { get; private set; }
    public ObjectHierarchy? ReferenceObjectHierarchy { get; private set; }

    // EF Core constructor
    private Item() { 
      Created = DateTime.UtcNow;
      Modified = DateTime.UtcNow;
    }

    public Item(string name, string details, int itemTypeId, int statusTypeId, 
                int rank = 0, int? parentId = null, 
                int? referenceFileSystemId = null, int? referenceObjectHierarchyId = null) {
      Name = name;
      Details = details;
      ItemTypeId = itemTypeId;
      StatusTypeId = statusTypeId;
      Rank = rank;
      ParentId = parentId;
      ReferenceFileSystemId = referenceFileSystemId;
      ReferenceObjectHierarchyId = referenceObjectHierarchyId;
      Created = DateTime.UtcNow;
      Modified = DateTime.UtcNow;
    }

    public void Update(string name, string details, int itemTypeId, int statusTypeId, int rank) {
      Name = name;
      Details = details;
      ItemTypeId = itemTypeId;
      StatusTypeId = statusTypeId;
      Rank = rank;
      Modified = DateTime.UtcNow;
    }

    public void UpdateStatus(int statusTypeId) {
      StatusTypeId = statusTypeId;
      Modified = DateTime.UtcNow;
    }

    public void UpdateRank(int rank) {
      Rank = rank;
      Modified = DateTime.UtcNow;
    }

    public void MoveTo(int? newParentId) {
      ParentId = newParentId;
      Modified = DateTime.UtcNow;
    }

    public void MarkCompleted() {
      Completed = DateTime.UtcNow;
      Modified = DateTime.UtcNow;
    }

    public void ClearCompleted() {
      Completed = null;
      Modified = DateTime.UtcNow;
    }

    public void SetReferences(int? fileSystemId = null, int? objectHierarchyId = null) {
      ReferenceFileSystemId = fileSystemId;
      ReferenceObjectHierarchyId = objectHierarchyId;
      Modified = DateTime.UtcNow;
    }
  }
}
