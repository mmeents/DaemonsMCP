using MediatR;
using DaemonsMCP.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Application.Models.Commands.DeleteModel {

  public record DeleteModelPropertyCommand(
    int ModelPropertyId
  ) : IRequest<bool>;

  public class DeleteModelPropertyCommandHandler : IRequestHandler<DeleteModelPropertyCommand, bool> {
    private readonly IModelPropertyRepository _modelPropertyRepository;

    public DeleteModelPropertyCommandHandler(IModelPropertyRepository modelPropertyRepository) {
      _modelPropertyRepository = modelPropertyRepository;
    }

    public async Task<bool> Handle(DeleteModelPropertyCommand request, CancellationToken cancellationToken) {
      var modelProperty = await _modelPropertyRepository.GetByIdAsync(request.ModelPropertyId, cancellationToken);
      if (modelProperty == null) return false;

      await _modelPropertyRepository.DeleteAsync(modelProperty.Id, cancellationToken);
      return true;
    }
  }
}
