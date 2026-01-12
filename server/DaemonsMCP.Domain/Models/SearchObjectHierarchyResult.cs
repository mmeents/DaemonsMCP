using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Models {
  public record SearchObjectHierarchyResult {
    public List<ObjectHierarchyNodeDto> Data { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNo { get; set; }
    public int PageSize { get; set; }
    public string? SearchTerm { get; set; } = string.Empty;
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

  }

    public class ObjectHierarchyNodeDto {
      public int Id { get; init; }
      public string IdentifierName { get; init; } = string.Empty;
      public string IdentifierTypeName { get; init; } = string.Empty;
      public int IdentifierTypeId { get; init; }
      public int? ParentId { get; init; }
      public string? ParentName { get; init; } = string.Empty;
      public string? ParentTypeName { get; init; } = string.Empty;  
      public int FileSystemNodeId { get; init; }
      public string RelativePath { get; init; } = string.Empty;
      public string FileName { get; init; } = string.Empty;
      public int LineStart { get; init; }
      public int LineEnd { get; init; }
      public DateTime IndexedAt { get; init; }
      public int ProjectId { get; init; }
    } 


}
