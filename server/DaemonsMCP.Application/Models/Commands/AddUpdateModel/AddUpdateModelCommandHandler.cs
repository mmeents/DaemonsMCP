using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Enums;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace DaemonsMCP.Application.Models.Commands.AddUpdateModel {

  public record AddUpdateModelCommand (
    int Id,
    int ProjectId,
    int? ParentId,
    int ModelTypeId,
    string Name,
    int Rank,
    string? Code = null
  ) : IRequest<ModelDto>;

  public class AddUpdateModelCommandHandler (
    IModelRepository repository, 
    IModelTypeRepository modelTypeRepository,
    IMediator mediator
  ) : IRequestHandler<AddUpdateModelCommand, ModelDto> {

    private readonly IModelRepository _repository = repository;
    private readonly IModelTypeRepository _modelTypeRepository = modelTypeRepository;
    private IMediator _mediator = mediator;

    public async Task<ModelDto> Handle(AddUpdateModelCommand request, CancellationToken cancellationToken) {
      Model? model;
      Model? parent = null;

      if (request.ParentId.HasValue) {
        parent = await _repository.GetByIdAsync(request.ParentId.Value, cancellationToken);
        if (parent == null) {
          throw new InvalidOperationException($"Parent model with id {request.ParentId.Value} not found");
        }
        var parentModelType = await _modelTypeRepository.GetByIdWithChildrenAsync(parent.ModelTypeId, 2, cancellationToken);
        if (parentModelType == null) { 
          throw new InvalidOperationException($"Parent model type with id {parent.ModelTypeId} not found");
        }
        if (request.ModelTypeId >= (int)Mte.DatabaseModel && request.ModelTypeId < (int)Mte.RootTemplate) {  // check valid parent within the model realm.
          if (!parentModelType.Children.Any(mt => mt.Id == request.ModelTypeId)) {
            throw new InvalidOperationException($"Model type with id {request.ModelTypeId} is not a valid child of parent model type with id {parent.ModelTypeId}");
          }
        } else if (request.ModelTypeId >= (int)Mte.RootTemplate && request.ModelTypeId <= (int)Mte.ClassTemplate) {  // check valid parent within the template realm.
          if (parent.ModelTypeId < (int)Mte.RootTemplate && parent.ModelTypeId > (int)Mte.ClassTemplate) {
            throw new InvalidOperationException($"Model type with id {request.ModelTypeId} is not a valid child of parent model type with id {parent.ModelTypeId}");
          }            
        }        
      }

      if (request.Id == 0) {
                
        var rank = request.Rank;
        if (rank == 0) {
          if (parent == null) { 
            rank = 1;
          }
          else { 
            rank = parent.Children.Count() + 1;
          }                  
        }

        // Create new model
        model = new Model(
          request.ProjectId,
          request.Name,
          request.ModelTypeId,
          rank,
          request.ParentId,
          request.Code);
        await _repository.AddAsync(model, cancellationToken);        

      } else {

        // Update existing model
        model = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (model == null) {
          throw new InvalidOperationException($"Model with id {request.Id} not found");
        }

        model.Update(
          request.Name,
          request.ModelTypeId,
          request.Rank,
          request.Code);

        if (request.ParentId != model.ParentId) {
          model.MoveTo(request.ParentId);
        }

        await _repository.UpdateAsync(model, cancellationToken);
      }           

      return model.ToDto();
    }
  }
  
}
