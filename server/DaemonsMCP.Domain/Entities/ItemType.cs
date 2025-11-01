namespace DaemonsMCP.Domain.Entities {
  public class ItemType {
    public int Id { get; private set; }
    public int? ParentId { get; private set; }
    public int Rank { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    // Navigation properties
    public ItemType? Parent { get; private set; }
    public ICollection<ItemType> Children { get; private set; } = new List<ItemType>();
    public ICollection<Item> Items { get; private set; } = new List<Item>();

    // EF Core constructor
    private ItemType() { }

    public ItemType(string name, string description, int rank = 0, int? parentId = null) {
      Name = name;
      Description = description;
      Rank = rank;
      ParentId = parentId;
    }

    public void Update(string name, string description, int rank, int? parentId = null) {
      Name = name;
      Description = description;
      Rank = rank;
      ParentId = parentId;
    }

    public void UpdateRank(int rank) {
      Rank = rank;
    }
  }
}
