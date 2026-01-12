using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Application.ModelTypes.Queries.GetAllModelTypes {

  public record GetAllModelTypesQuery() : IRequest<List<ModelTypeDto>>;

  public class GetAllModelTypesQueryHandler : IRequestHandler<GetAllModelTypesQuery, List<ModelTypeDto>> {
    private readonly IModelTypeRepository _repository;

    public GetAllModelTypesQueryHandler(IModelTypeRepository repository) {
      _repository = repository;
    }

    public async Task<List<ModelTypeDto>> Handle(GetAllModelTypesQuery request, CancellationToken cancellationToken) {
      var modelTypes = await _repository.GetAllAsync(cancellationToken);
      return modelTypes.Select(mt => mt.ToDto()).ToList();
    }
  }
}
