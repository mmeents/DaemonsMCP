using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Domain.Entities;
using MediatR;

namespace DaemonsMCP.Application.ItemTypes.Commands.AddUpdateItemType;

public class AddUpdateItemTypeCommandHandler : IRequestHandler<AddUpdateItemTypeCommand, ItemTypeDto> {
  private readonly IItemTypeRepository _repository;

  public AddUpdateItemTypeCommandHandler(IItemTypeRepository repository) {
    _repository = repository;
  }

  public async Task<ItemTypeDto> Handle(AddUpdateItemTypeCommand request, CancellationToken cancellationToken) {
    ItemType itemType;
    
    if (request.Id == 0) {
      // Create new item type
      itemType = new ItemType(request.Name, request.Description, request.Rank, request.ParentId);
      await _repository.AddAsync(itemType, cancellationToken);
    } else {
      // Update existing item type
      itemType = await _repository.GetByIdAsync(request.Id, cancellationToken);
      if (itemType == null) {
        throw new InvalidOperationException($"ItemType with id {request.Id} not found");
      }

      itemType.Update(request.Name, request.Description, request.Rank, request.ParentId);
      await _repository.UpdateAsync(itemType, cancellationToken);
    }

    await _repository.SaveChangesAsync(cancellationToken);

    return new ItemTypeDto {
      Id = itemType.Id,
      ParentId = itemType.ParentId,
      Rank = itemType.Rank,
      Name = itemType.Name,
      Description = itemType.Description,
      Children = new List<ItemTypeDto>()
    };
  }
}
