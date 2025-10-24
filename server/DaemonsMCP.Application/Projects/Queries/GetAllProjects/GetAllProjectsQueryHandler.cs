using MediatR;
using DaemonsMCP.Domain.Repositories;

namespace DaemonsMCP.Application.Projects.Queries.GetAllProjects;

public class GetAllProjectsQueryHandler
    : IRequestHandler<GetAllProjectsQuery, List<ProjectDto>> {
  private readonly IProjectRepository _projectRepository;

  public GetAllProjectsQueryHandler(IProjectRepository projectRepository) {
    _projectRepository = projectRepository;
  }

  public async Task<List<ProjectDto>> Handle(
      GetAllProjectsQuery request,
      CancellationToken cancellationToken) {
    var projects = await _projectRepository.GetAllAsync(cancellationToken);

    return projects.Select(p => new ProjectDto(
        p.Id,
        p.Name,
        p.Description,
        p.RootPath,
        p.CreatedAt
    )).ToList();
  }
}