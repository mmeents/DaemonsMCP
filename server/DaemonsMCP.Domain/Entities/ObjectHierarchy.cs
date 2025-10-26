using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Entities;
 
public class ObjectHierarchy {
  public int Id { get; set; }
  public int? ParentId { get; set; }  // self referencing, 0 for root
  public int ProjectId { get; set; } // FK to Project
  public int FileSystemNodeId { get; set; } // FK to FileSystemNode
  public int IdentifierId { get; set; } // FK to Identifier
  public int IdentifierTypeId { get; set; } // FK to IdentifierType
  public int LineStart { get; set; }
  public int LineEnd { get; set;}
  public DateTime IndexedAt { get; set; }


  // Navigation properties
  public Project Project { get; private set; } = null!;
  public FileSystemNode FileSystemNode { get; set; } = null!;
  public ObjectHierarchy? Parent { get; set; }
  public ICollection<ObjectHierarchy> Children { get; set; } = new List<ObjectHierarchy>();
  public Identifier Identifier { get; set; } = null!;
  public IdentifierType IdentifierType { get; set; } = null!;
  

  // EF Core constructor
  private ObjectHierarchy() { }

  // Factory method
    public static ObjectHierarchy Create(
        int? parentId,
        int projectId,
        int fileSystemNodeId,
        int identifierId,
        int identifierTypeId,
        int lineStart,
        int lineEnd) {
        return new ObjectHierarchy {
        ParentId = parentId,
        ProjectId = projectId,
        FileSystemNodeId = fileSystemNodeId,
        IdentifierId = identifierId,
        IdentifierTypeId = identifierTypeId,
        LineStart = lineStart,
        LineEnd = lineEnd,
        IndexedAt = DateTime.UtcNow
        };
    }



}

