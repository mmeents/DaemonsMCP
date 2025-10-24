using MediatR;

namespace DaemonsMCP.Application.Projects.Queries.GetAllProjects;

public record GetAllProjectsQuery() : IRequest<List<ProjectDto>>;

public record ProjectDto(
    int Id,
    string Name,
    string Description,
    string RootPath,
    DateTime CreatedAt
);
