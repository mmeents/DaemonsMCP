using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;

namespace DaemonsMCP.Application.ItemTypes.Queries.GetStatusTypes {
  public class GetStatusTypeQueryHandler : IRequestHandler<GetStatusTypesQuery, List<ItemTypeDto>> {
    private readonly IItemTypeRepository _repository;
    public GetStatusTypeQueryHandler(
      IItemTypeRepository repository
    ) { 
      _repository = repository;
    }
    public async Task<List<ItemTypeDto>> Handle(GetStatusTypesQuery request, CancellationToken cancellationToken) {
      var types = await _repository.GetStatusTypes(cancellationToken);
      List<ItemTypeDto> dtos = new List<ItemTypeDto>();
      foreach (var type in types)
      {
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
