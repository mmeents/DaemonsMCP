using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Domain.Entities;
using MediatR;

namespace DaemonsMCP.Application.Items.Queries.SearchItems;

public class SearchItemsQueryHandler : IRequestHandler<SearchItemsQuery, List<ItemDto>> {
  private readonly IItemRepository _repository;
  private readonly IItemTypeRepository _itemTypeRepository;

  public SearchItemsQueryHandler(
    IItemRepository repository,
    IItemTypeRepository itemTypeRepository) {
    _repository = repository;
    _itemTypeRepository = itemTypeRepository;
  }

  public async Task<List<ItemDto>> Handle(SearchItemsQuery request, CancellationToken cancellationToken) {
    var items = await _repository.SearchAsync(
      request.NameContains,
      request.DetailsContains,
      request.TypeId,
      request.StatusId,
      request.ParentId,
      cancellationToken);

    var dtos = new List<ItemDto>();
    foreach (var item in items) {
      var dto = await MapToDto(item, request.MaxDepth, cancellationToken);
      dtos.Add(dto);
    }

    return dtos;
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

    if (maxDepth > 0) {
      var children = await _repository.GetByParentIdAsync(item.Id, cancellationToken);
      foreach (var child in children) {
        var childDto = await MapToDto(child, maxDepth - 1, cancellationToken);
        dto.Children.Add(childDto);
      }
    }

    return dto;
  }
}
