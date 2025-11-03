using MediatR;

namespace DaemonsMCP.Domain.Models;

public record GetReadmeQuery() : IRequest<List<ItemDto>>;

public record SearchItemsQuery(
  int? ParentId = null,
  string? NameContains = null,
  string? DetailsContains = null,
  int? TypeId = null,
  int? StatusId = null,
  int MaxDepth = 1
) : IRequest<List<ItemDto>>;
