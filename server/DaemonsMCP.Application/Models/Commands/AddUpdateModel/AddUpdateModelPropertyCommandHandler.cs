using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using MediatR;

namespace DaemonsMCP.Application.Models.Commands.AddUpdateModel {

  public record AddUpdateModelPropertyCommand(
    int Id,
    int ModelId,
    string PropertyKey,
    string? PropertyValue = null,
    int? PropertyValueTypeId = null,
    int? PropertyEditorTypeId = null
  ) : IRequest<ModelPropertyDto>;

  public class AddUpdateModelPropertyCommandHandler : IRequestHandler<AddUpdateModelPropertyCommand, ModelPropertyDto> {
    private readonly IModelPropertyRepository _repository;
    private readonly IModelTypeRepository _modelTypeRepository;

    public AddUpdateModelPropertyCommandHandler(
      IModelPropertyRepository repository,
      IModelTypeRepository modelTypeRepository) {
      _repository = repository;
      _modelTypeRepository = modelTypeRepository;
    }

    public async Task<ModelPropertyDto> Handle(AddUpdateModelPropertyCommand request, CancellationToken cancellationToken) {
      ModelProperty? property;
      int id = 0;

      if (request.Id == 0) {
        // Create new property
        property = new ModelProperty(
          request.ModelId,
          request.PropertyKey,
          request.PropertyValue,
          request.PropertyValueTypeId,
          request.PropertyEditorTypeId);

        id = await _repository.AddAsync(property, cancellationToken);
      } else {
        // Update existing property
        id = request.Id;
        property = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (property == null) {
          throw new InvalidOperationException($"ModelProperty with id {request.Id} not found");
        }

        property.Update(request.PropertyValue, request.PropertyValueTypeId, request.PropertyEditorTypeId);
        await _repository.UpdateAsync(property, cancellationToken);
      }

      property = await _repository.GetByIdAsync(id, cancellationToken);

      // Return the DTO      
      return property?.ToDto() ?? throw new Exception("Property not found after update");
    }
  }
}
