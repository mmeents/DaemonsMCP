using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using MediatR;

namespace DaemonsMCP.Application.Models.Queries.GetModelById {
  public record GetModelByIdQuery(
    int Id,
    int MaxDepth = 1,
    bool IncludeProperties = true
  ) : IRequest<ModelDto?>;

  public class GetModelByIdQueryHandler : IRequestHandler<GetModelByIdQuery, ModelDto?> {
    private readonly IModelRepository _modelRepository;
    private readonly IModelTypeRepository _modelTypeRepository;
    private readonly IModelPropertyRepository _propertyRepository;

    public GetModelByIdQueryHandler(
      IModelRepository modelRepository,
      IModelTypeRepository modelTypeRepository,
      IModelPropertyRepository propertyRepository) {
      _modelRepository = modelRepository;
      _modelTypeRepository = modelTypeRepository;
      _propertyRepository = propertyRepository;
    }

    public async Task<ModelDto?> Handle(GetModelByIdQuery request, CancellationToken cancellationToken) {
      var model = request.MaxDepth > 0
        ? await _modelRepository.GetByIdWithChildrenAsync(request.Id, request.MaxDepth, cancellationToken)
        : await _modelRepository.GetByIdAsync(request.Id, cancellationToken);

      if (model == null) return null;

      return await MapToDto(model, request.IncludeProperties, cancellationToken);
    }

    private async Task<ModelDto> MapToDto(Domain.Entities.Model model, bool includeProperties, CancellationToken cancellationToken) {
      var modelType = await _modelTypeRepository.GetByIdAsync(model.ModelTypeId, cancellationToken);

      var properties = includeProperties
        ? await _propertyRepository.GetByModelIdAsync(model.Id, cancellationToken)
        : new List<Domain.Entities.ModelProperty>();

      return model.ToDto() with {
        ModelTypeName = modelType?.Name ?? string.Empty,
        Properties = properties.Select(p => p.ToDto()).ToList(),
        Children = model.Children?.Select(c => MapToDto(c, includeProperties, cancellationToken).Result).ToList() ?? new()
      };
    }
  }
}
