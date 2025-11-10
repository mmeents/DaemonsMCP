using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Application.Items.Commands.AddUpdateItem {
  public record AddUpdateItemRequest(
  int Id,
  int? ParentId,
  int ItemTypeId,
  int StatusTypeId,
  int Rank,
  string Name,
  string Details,
  int? ReferenceFileSystemId = null,
  int? ReferenceObjectHierarchyId = null);

  public record AddUpdateItemTypeRequest(
    int Id,
    string Name,
    string Description,
    int Rank,
    int? ParentId = null);

}
