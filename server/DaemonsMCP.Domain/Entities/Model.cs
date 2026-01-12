using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Entities {
  public class Model {
    public int Id { get; private set; }
    public int ProjectId { get; private set; }
    public int? ParentId { get; private set; }
    public int ModelTypeId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Rank { get; private set; }
    public string? Code { get; private set; }  // Preserve original SQL/code
    public DateTime CreatedDate { get; private set; }
    public DateTime ModifiedDate { get; private set; }

    // Navigation properties
    public Project Project { get; private set; } = null!;
    public Model? Parent { get; private set; }
    public ICollection<Model> Children { get; private set; } = [];
    public ModelType ModelType { get; private set; } = null!;
    public ICollection<ModelProperty> Properties { get; private set; } = [];

    // EF Core constructor
    private Model() {
      CreatedDate = DateTime.UtcNow;
      ModifiedDate = DateTime.UtcNow;
    }

    public Model(int projectId, string name, int modelTypeId, int rank = 0,
                 int? parentId = null, string? code = null) {
      ProjectId = projectId;
      Name = name;
      ModelTypeId = modelTypeId;
      Rank = rank;
      ParentId = parentId;
      Code = code;
      CreatedDate = DateTime.UtcNow;
      ModifiedDate = DateTime.UtcNow;
    }

    public void Update(string name, int modelTypeId, int rank, string? code = null) {
      Name = name;
      ModelTypeId = modelTypeId;
      Rank = rank;
      Code = code;
      ModifiedDate = DateTime.UtcNow;
    }

    public void UpdateRank(int rank) {
      Rank = rank;
      ModifiedDate = DateTime.UtcNow;
    }

    public void MoveTo(int? newParentId) {
      ParentId = newParentId;
      ModifiedDate = DateTime.UtcNow;
    }

    public void UpdateCode(string? code) {
      Code = code;
      ModifiedDate = DateTime.UtcNow;
    }
  }
}
