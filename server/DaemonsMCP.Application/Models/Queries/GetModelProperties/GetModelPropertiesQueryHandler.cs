using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Application.Models.Queries.GetModelProperties {

  public record GetModelPropertiesQuery(
    int ModelId
  ) : IRequest<List<ModelPropertyDto>>;

  public class GetModelPropertiesQueryHandler : IRequestHandler<GetModelPropertiesQuery, List<ModelPropertyDto>> {
    private readonly IModelRepository _repository;

    public GetModelPropertiesQueryHandler(IModelRepository repository) {
      _repository = repository;
    }

    public async Task<List<ModelPropertyDto>> Handle(GetModelPropertiesQuery request, CancellationToken cancellationToken) {
      var model = await _repository.GetByIdWithPropertiesAsync(request.ModelId, cancellationToken);
      if (model == null) {
        return new List<ModelPropertyDto>();
      }

      return model.Properties.Select(p => p.ToDto()).ToList();
    }
  }
}
