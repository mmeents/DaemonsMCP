using MediatR;

namespace DaemonsMCP.Domain.Models;

public record GetItemByIdQuery(
  int ItemId,
  int MaxDepth = 1
) : IRequest<ItemDto?>;
