using MediatR;

namespace DaemonsMCP.Domain.Models;

public record AddUpdateItemCommand(
  int Id,
  int? ParentId,
  int ItemTypeId,
  int StatusTypeId,
  int Rank,
  string Name,
  string Details,
  int? ReferenceFileSystemId = null,
  int? ReferenceObjectHierarchyId = null
) : IRequest<ItemDto>;




