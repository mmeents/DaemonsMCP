using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Domain.Entities;
using MediatR;

namespace DaemonsMCP.Application.Items.Commands.AddUpdateItem;

public class AddUpdateItemCommandHandler : IRequestHandler<AddUpdateItemCommand, ItemDto> {
  private readonly IItemRepository _repository;
  private readonly IItemTypeRepository _itemTypeRepository;

  public AddUpdateItemCommandHandler(
    IItemRepository repository,
    IItemTypeRepository itemTypeRepository) {
    _repository = repository;
    _itemTypeRepository = itemTypeRepository;
  }

  public async Task<ItemDto> Handle(AddUpdateItemCommand request, CancellationToken cancellationToken) {
    Item? item;
    
    if (request.Id == 0) {
      // Create new item
      item = new Item(
        request.Name,
        request.Details,
        request.ItemTypeId,
        request.StatusTypeId,
        request.Rank,
        request.ParentId,
        request.ReferenceFileSystemId,
        request.ReferenceObjectHierarchyId);
      
      await _repository.AddAsync(item, cancellationToken);
    } else {
      // Update existing item
      item = await _repository.GetByIdAsync(request.Id, cancellationToken);
      if (item == null) {
        throw new InvalidOperationException($"Item with id {request.Id} not found");
      }

      item.Update(
        request.Name,
        request.Details,
        request.ItemTypeId,
        request.StatusTypeId,
        request.Rank);

      if (request.ParentId != item.ParentId) {
        item.MoveTo(request.ParentId);
      }

      if (request.ReferenceFileSystemId != item.ReferenceFileSystemId ||
          request.ReferenceObjectHierarchyId != item.ReferenceObjectHierarchyId) {
        item.SetReferences(request.ReferenceFileSystemId, request.ReferenceObjectHierarchyId);
      }

      await _repository.UpdateAsync(item, cancellationToken);
    }

    await _repository.SaveChangesAsync(cancellationToken);

    // Return the DTO
    var itemType = await _itemTypeRepository.GetByIdAsync(item.ItemTypeId, cancellationToken);
    var statusType = await _itemTypeRepository.GetByIdAsync(item.StatusTypeId, cancellationToken);

    return new ItemDto {
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
  }
}
