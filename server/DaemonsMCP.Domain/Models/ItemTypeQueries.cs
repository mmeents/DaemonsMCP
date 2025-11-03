using MediatR;
using DaemonsMCP.Domain.Entities;

namespace DaemonsMCP.Domain.Models;

public record GetAllItemTypesQuery() : IRequest<List<ItemTypeDto>>;

public record GetStatusTypesQuery() : IRequest<List<ItemTypeDto>>;

public record GetItemTypesQuery() : IRequest<List<ItemTypeDto>>;

public record GetItemTypeByNameQuery(string Name) : IRequest<ItemTypeDto?>;

public record AddUpdateItemTypeCommand(
  int Id,
  string Name,
  string Description,
  int Rank,
  int? ParentId = null
) : IRequest<ItemTypeDto>;
