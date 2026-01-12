using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using MediatR;

namespace DaemonsMCP.Application.ModelTypes.Queries.GetValidChildTypes {

  public record GetValidChildTypesQuery(
    int ParentModelTypeId
  ) : IRequest<List<ModelTypeDto>>;

  public class GetValidChildTypesQueryHandler : IRequestHandler<GetValidChildTypesQuery, List<ModelTypeDto>> {
    private readonly IModelTypeRepository _modelTypeRepository;

    public GetValidChildTypesQueryHandler(IModelTypeRepository modelTypeRepository) {
      _modelTypeRepository = modelTypeRepository;
    }

    public async Task<List<ModelTypeDto>> Handle(GetValidChildTypesQuery request, CancellationToken cancellationToken) {
      var modelTypes = await _modelTypeRepository.GetByIdWithChildrenAsync(request.ParentModelTypeId, 2, cancellationToken);
      return modelTypes?.Children.Select(mt => mt.ToDto()).ToList() ?? new List<ModelTypeDto>();
    }
  }
}
