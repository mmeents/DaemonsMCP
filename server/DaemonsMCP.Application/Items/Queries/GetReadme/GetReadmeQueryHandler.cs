using DaemonsMCP.Application.Items.Queries.GetReadme;
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

namespace DaemonsMCP.Application.Items.Queries.GetReadme {
  public class GetReadmeQueryHandler : IRequestHandler<GetReadmeQuery, List<ItemDto>> {
    private readonly IItemRepository _repository;
    private readonly IItemTypeRepository _itemTypeRepository;
    public GetReadmeQueryHandler(
      IItemRepository repository,
      IItemTypeRepository itemTypeRepository) {
      _repository = repository;
      _itemTypeRepository = itemTypeRepository;
    }

    public async Task<List<ItemDto>> Handle(GetReadmeQuery request, CancellationToken cancellationToken) { 
       
      var items = await _repository.SearchAsync(
        null,
        null,
        Cx.ItemTypeIdReadme,
        null,
        null,
        cancellationToken);

      var dtos = new List<ItemDto>();
      foreach (var item in items) {
        var dto = await MapToDto(item, 10, cancellationToken);
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
}
