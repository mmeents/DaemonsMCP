using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Application.ObjectHierarchy.Queries.SearchObjectHierarchy {
  public record SearchObjectHierarchyQuery(
    int ProjectId,               // Required - scope to project
    string? SearchTerm,          // Optional - search identifier names (partial match)
    int? IdentifierTypeId,       // Optional - filter by type (Class, Method, etc.)
    int? FileSystemNodeId,       // Optional - limit to specific file
    int? ParentId,               // Optional - find children of specific node
    int PageNo = 1,
    int PageSize = 20
  ) : IRequest<SearchObjectHierarchyResult>;

}
