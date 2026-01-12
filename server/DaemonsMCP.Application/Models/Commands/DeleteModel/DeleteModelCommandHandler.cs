using MediatR;
using DaemonsMCP.Domain.Repositories;


namespace DaemonsMCP.Application.Models.Commands.DeleteModel {

  public record DeleteModelCommand(
    int Id
  ) : IRequest<bool>;

  internal class DeleteModelCommandHandler : IRequestHandler<DeleteModelCommand, bool> {
    private readonly IModelRepository _repository;

    public DeleteModelCommandHandler(IModelRepository repository) {
      _repository = repository;
    }

    public async Task<bool> Handle(DeleteModelCommand request, CancellationToken cancellationToken) {
      var model = await _repository.GetByIdAsync(request.Id, cancellationToken);
      if (model == null) {
        return false;
      }

      await _repository.DeleteAsync(model.Id, cancellationToken);
      return true;
    }
  }
}
