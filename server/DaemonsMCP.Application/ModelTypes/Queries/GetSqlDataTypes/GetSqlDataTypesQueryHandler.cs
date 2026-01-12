using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using MediatR;

namespace DaemonsMCP.Application.ModelTypes.Queries.GetSqlDataTypes {
  public record GetSqlDataTypesQuery() : IRequest<List<ModelTypeDto>>;

  public class GetSqlDataTypesQueryHandler : IRequestHandler<GetSqlDataTypesQuery, List<ModelTypeDto>> {
    private readonly IModelTypeRepository _repository;

    public GetSqlDataTypesQueryHandler(IModelTypeRepository repository) {
      _repository = repository;
    }

    public async Task<List<ModelTypeDto>> Handle(GetSqlDataTypesQuery request, CancellationToken cancellationToken) {
      var sqlTypes = await _repository.GetSqlDataTypesAsync(cancellationToken);
      return sqlTypes.Select(e => e.ToDto()).ToList();
    }
  }
}
