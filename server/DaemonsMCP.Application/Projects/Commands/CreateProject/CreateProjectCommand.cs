using MediatR;

namespace DaemonsMCP.Application.Projects.Commands.CreateProject;

public record CreateProjectCommand(
    string Name,
    string Description,
    string RootPath
) : IRequest<CreateProjectResult>;

public record CreateProjectResult(
    int Id,
    string Name,
    string Description,
    string RootPath,
    DateTime CreatedAt
);
