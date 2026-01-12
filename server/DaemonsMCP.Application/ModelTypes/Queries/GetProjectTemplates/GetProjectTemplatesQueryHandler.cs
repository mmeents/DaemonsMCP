using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using MediatR;


namespace DaemonsMCP.Application.ModelTypes.Queries.GetProjectTemplates {
  public record GetProjectTemplatesQuery() : IRequest<List<ModelTypeDto>>;

  public class GetProjectTemplatesQueryHandler : IRequestHandler<GetProjectTemplatesQuery, List<ModelTypeDto>> {
    private readonly IModelTypeRepository _repository;

    public GetProjectTemplatesQueryHandler(IModelTypeRepository repository) {
      _repository = repository;
    }

    public async Task<List<ModelTypeDto>> Handle(GetProjectTemplatesQuery request, CancellationToken cancellationToken) {
      var templates = await _repository.GetProjectTemplatesAsync(cancellationToken);
      return templates.Select(e => e.ToDto()).ToList();
    }
  }
}
