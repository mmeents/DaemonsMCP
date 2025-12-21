using DaemonsMCP.Domain.Constants;
using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Application.Items.Queries.GetReadmeItems {

  public record GetReadmeItemsQuery() : IRequest<List<ReadmeItemDto>>;

  public class GetReadmeItemsQueryHandler : IRequestHandler<GetReadmeItemsQuery, List<ReadmeItemDto>> {
    private readonly IItemRepository _repository;
    private readonly IItemTypeRepository _itemTypeRepository;
    public GetReadmeItemsQueryHandler(
      IItemRepository repository,
      IItemTypeRepository itemTypeRepository) {
      _repository = repository;
      _itemTypeRepository = itemTypeRepository;
    }

    public async Task<List<ReadmeItemDto>> Handle(GetReadmeItemsQuery request, CancellationToken cancellationToken) { 
       
      var items = await _repository.SearchAsync(
        null,
        null,
        Cx.ItemTypeIdReadme,
        null,
        null,
        cancellationToken);

      var dtos = new List<ReadmeItemDto>();
      foreach (var item in items) {
        var dto = await MapToDto(item, 10, cancellationToken);
        dtos.Add(dto);
      }

      return dtos;
    }

    private async Task<ReadmeItemDto> MapToDto(Item item, int maxDepth, CancellationToken cancellationToken) {
      var itemType = await _itemTypeRepository.GetByIdAsync(item.ItemTypeId, cancellationToken);
      var statusType = await _itemTypeRepository.GetByIdAsync(item.StatusTypeId, cancellationToken);

      var dto = new ReadmeItemDto {
        Id = item.Id,
        Name = item.Name,
        Details = item.Details,
        Modified = item.Modified,
        Children = new List<ReadmeItemDto>()
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
}
