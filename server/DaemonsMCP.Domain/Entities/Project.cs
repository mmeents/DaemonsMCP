namespace DaemonsMCP.Domain.Entities {
  public class Project {
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string RootPath { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    // EF Core needs a parameterless constructor (can be private)
    private Project() { }

    public Project(string name, string description, string rootPath) {
      Name = name;
      Description = description;
      RootPath = rootPath;
      CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string name, string description, string rootPath) {
      Name = name;
      Description = description;
      RootPath = rootPath;
    }
  }
}
