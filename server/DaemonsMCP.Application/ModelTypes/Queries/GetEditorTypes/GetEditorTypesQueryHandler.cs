using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using MediatR;


namespace DaemonsMCP.Application.ModelTypes.Queries.GetEditorTypes {

  public record GetEditorTypesQuery() : IRequest<List<ModelTypeDto>>;


  public class GetEditorTypesQueryHandler : IRequestHandler<GetEditorTypesQuery, List<ModelTypeDto>> {
    private readonly IModelTypeRepository _repository;

    public GetEditorTypesQueryHandler(IModelTypeRepository repository) {
      _repository = repository;
    }

    public async Task<List<ModelTypeDto>> Handle(GetEditorTypesQuery request, CancellationToken cancellationToken) {
      var editors = await _repository.GetEditorTypesAsync(cancellationToken);
      return editors.Select(e => e.ToDto()).ToList();
    }
  }
}
