using DaemonsMCP.Application.FileSystem.Commands.SyncProjectFileSystem;
using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Repositories;
using MediatR;

namespace DaemonsMCP.Application.Projects.Commands.CreateProject;

public class CreateProjectCommandHandler
    : IRequestHandler<CreateProjectCommand, CreateProjectResult> {
  private readonly IProjectRepository _projectRepository;
  private readonly IMediator _mediator;

  public CreateProjectCommandHandler(IProjectRepository projectRepository, IMediator mediator) {
    _projectRepository = projectRepository;
    _mediator = mediator;
  }

  public async Task<CreateProjectResult> Handle(
      CreateProjectCommand request,
      CancellationToken cancellationToken) {
    // Business validation
    if (await _projectRepository.ExistsAsync(request.Name, cancellationToken)) {
      throw new InvalidOperationException($"Project with name '{request.Name}' already exists.");
    }

    // Create domain entity
    var project = new Project(
        request.Name,
        request.Description,
        request.RootPath
    );

    // Save to repository
    await _projectRepository.AddAsync(project, cancellationToken);
    await _projectRepository.SaveChangesAsync(cancellationToken);

    // Sync filesystem after creating project
    await _mediator.Send(new SyncProjectFileSystemCommand(project.Id), cancellationToken);


    // Return result
    return new CreateProjectResult(
        project.Id,
        project.Name,
        project.Description,
        project.RootPath,
        project.CreatedAt
    );
  }
}
