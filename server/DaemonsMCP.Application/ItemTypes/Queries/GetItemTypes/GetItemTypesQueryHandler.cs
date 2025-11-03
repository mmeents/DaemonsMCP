using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Application.ItemTypes.Queries.GetItemTypes {
  public class GetItemTypesQueryHandler : IRequestHandler<GetItemTypesQuery, List<ItemTypeDto>> {
    private readonly IItemTypeRepository _repository;
    public GetItemTypesQueryHandler(
      IItemTypeRepository repository
    ) { 
      _repository = repository;
    }

    public async Task<List<ItemTypeDto>> Handle(GetItemTypesQuery request, CancellationToken cancellationToken) {
      var types = await _repository.GetItemTypes(cancellationToken);
      List<ItemTypeDto> dtos = new List<ItemTypeDto>();
      foreach (var type in types) {
        dtos.Add(new ItemTypeDto {
          Id = type.Id,
          ParentId = type.ParentId,
          Rank = type.Rank,
          Name = type.Name,
          Description = type.Description,
          Children = new List<ItemTypeDto>()
        });
      }
      return dtos;
    }
  }
}
