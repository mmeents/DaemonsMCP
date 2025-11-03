using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Domain.Entities;
using MediatR;

namespace DaemonsMCP.Application.Items.Queries.GetItemById;

public class GetItemByIdQueryHandler : IRequestHandler<GetItemByIdQuery, ItemDto?> {
  private readonly IItemRepository _repository;
  private readonly IItemTypeRepository _itemTypeRepository;

  public GetItemByIdQueryHandler(
    IItemRepository repository,
    IItemTypeRepository itemTypeRepository) {
    _repository = repository;
    _itemTypeRepository = itemTypeRepository;
  }

  public async Task<ItemDto?> Handle(GetItemByIdQuery request, CancellationToken cancellationToken) {
    var item = await _repository.GetByIdWithChildrenAsync(request.ItemId, request.MaxDepth, cancellationToken);
    if (item == null) return null;
    
    return await MapToDto(item, request.MaxDepth, cancellationToken);
  }

  private async Task<ItemDto> MapToDto(Item item, int maxDepth, CancellationToken cancellationToken) {
    var itemType = await _itemTypeRepository.GetByIdAsync(item.ItemTypeId, cancellationToken);
    var statusType = await _itemTypeRepository.GetByIdAsync(item.StatusTypeId, cancellationToken);

    var dto = new ItemDto {
      Id = item.Id,
      ParentId = item.ParentId,
      ItemTypeId = item.ItemTypeId,
      ItemTypeName = itemType?.Name ?? string.Empty,
      StatusTypeId = item.StatusTypeId,
      StatusTypeName = statusType?.Name ?? string.Empty,
      Rank = item.Rank,
      Name = item.Name,
      Details = item.Details,
      Created = item.Created,
      Modified = item.Modified,
      Completed = item.Completed,
      ReferenceFileSystemId = item.ReferenceFileSystemId,
      ReferenceObjectHierarchyId = item.ReferenceObjectHierarchyId,
      Children = new List<ItemDto>()
    };

    if (maxDepth > 0 && item.Children.Any()) {
      foreach (var child in item.Children) {
        var childDto = await MapToDto(child, maxDepth - 1, cancellationToken);
        dto.Children.Add(childDto);
      }
    }

    return dto;
  }
}
