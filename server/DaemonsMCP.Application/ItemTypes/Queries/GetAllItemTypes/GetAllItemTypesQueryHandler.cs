using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Domain.Entities;
using MediatR;

namespace DaemonsMCP.Application.ItemTypes.Queries.GetAllItemTypes;

public class GetAllItemTypesQueryHandler : IRequestHandler<GetAllItemTypesQuery, List<ItemTypeDto>> {
  private readonly IItemTypeRepository _repository;

  public GetAllItemTypesQueryHandler(IItemTypeRepository repository) {
    _repository = repository;
  }

  public async Task<List<ItemTypeDto>> Handle(GetAllItemTypesQuery request, CancellationToken cancellationToken) {
    var itemTypes = await _repository.GetAllAsync(cancellationToken);
    
    return itemTypes.Select(it => new ItemTypeDto {
      Id = it.Id,
      ParentId = it.ParentId,
      Rank = it.Rank,
      Name = it.Name,
      Description = it.Description,
      Children = new List<ItemTypeDto>()
    }).ToList();
  }
}
