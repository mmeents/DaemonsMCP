using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Application.Models.Queries.SearchModels {

  public record SearchModelsQuery(
    int ProjectId,
    int? ParentId = null,
    int? ModelTypeId = null,
    string? NameFilter = null,
    int MaxDepth = 1,
    bool IncludeProperties = false
  ) : IRequest<List<ModelDto>>;

  public class SearchModelsQueryHandler : IRequestHandler<SearchModelsQuery, List<ModelDto>> {
    private readonly IModelRepository _modelRepository;
    private readonly IModelTypeRepository _modelTypeRepository;
    private readonly IModelPropertyRepository _propertyRepository;

    public SearchModelsQueryHandler(
      IModelRepository modelRepository,
      IModelTypeRepository modelTypeRepository,
      IModelPropertyRepository propertyRepository) {
      _modelRepository = modelRepository;
      _modelTypeRepository = modelTypeRepository;
      _propertyRepository = propertyRepository;
    }

    public async Task<List<ModelDto>> Handle(SearchModelsQuery request, CancellationToken cancellationToken) {
      var models = await _modelRepository.SearchAsync(
        request.ProjectId,
        request.ParentId,
        request.ModelTypeId,
        request.NameFilter,
        cancellationToken);

      var dtos = new List<ModelDto>();
      foreach (var model in models) {
        var dto = await MapToDto(model, request.MaxDepth, request.IncludeProperties, cancellationToken);
        dtos.Add(dto);
      }

      return dtos;
    }

    private async Task<ModelDto> MapToDto(Domain.Entities.Model model, int maxDepth, bool includeProperties, CancellationToken cancellationToken) {
      var modelType = await _modelTypeRepository.GetByIdAsync(model.ModelTypeId, cancellationToken);

      var properties = includeProperties
        ? await _propertyRepository.GetByModelIdAsync(model.Id, cancellationToken)
        : new List<Domain.Entities.ModelProperty>();

      var dto = new ModelDto {
        Id = model.Id,
        ProjectId = model.ProjectId,
        ParentId = model.ParentId,
        ModelTypeId = model.ModelTypeId,
        ModelTypeName = modelType?.Name ?? string.Empty,
        Name = model.Name,
        Rank = model.Rank,
        Code = model.Code,
        CreatedDate = model.CreatedDate,
        ModifiedDate = model.ModifiedDate,
        Properties = properties.Select(p => p.ToDto()).ToList(),
        Children = new List<ModelDto>()
      };

      // Recursively load children if maxDepth allows
      if (maxDepth > 0) {
        var children = await _modelRepository.GetByParentIdAsync(model.Id, cancellationToken);
        foreach (var child in children) {
          var childDto = await MapToDto(child, maxDepth - 1, includeProperties, cancellationToken);
          dto.Children.Add(childDto);
        }
      }

      return dto;
    }

  }
}
