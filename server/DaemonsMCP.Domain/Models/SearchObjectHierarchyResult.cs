using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Models {
  public record SearchObjectHierarchyResult {
    public List<ObjectHierarchyNodeDto> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNo { get; set; }
    public int PageSize { get; set; }
    public string? SearchTerm { get; set; } = string.Empty;
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

  }

    public class ObjectHierarchyNodeDto {
      public int Id { get; init; }
      public string IdentifierName { get; init; }
      public string IdentifierTypeName { get; init; }
      public int IdentifierTypeId { get; init; }
      public int? ParentId { get; init; }
      public string? ParentName { get; init; }  
      public string? ParentTypeName { get; init; }  
      public int FileSystemNodeId { get; init; }
      public string RelativePath { get; init; }     
      public string FileName { get; init; }
      public int LineStart { get; init; }  
      public int LineEnd { get; init; }    
      public DateTime IndexedAt { get; init; }
      public int ProjectId { get; init; }
    } 


}
