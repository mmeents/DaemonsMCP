using DaemonsMCP.Application.Projects.Queries.GetAllProjects;
using DaemonsMCP.Application.Items.Queries.GetReadmeItems;
using DaemonsMCP.Domain.Models;
using MediatR;
using DaemonsMCP.Domain.Constants;

namespace DaemonsMCP.Application.Readme.Queries.GetReadme {
  
  public record GetReadmeQuery(): IRequest<ReadmeResponse>;

  public class GetReadmeQueryHandler : IRequestHandler<GetReadmeQuery, ReadmeResponse> {    
    private readonly IMediator _mediator;
    public GetReadmeQueryHandler(IMediator mediator) {
      _mediator = mediator;
    }

    public async Task<ReadmeResponse> Handle(GetReadmeQuery request, CancellationToken cancellationToken) {
                
      // Get readme items
      var readmeQueryItems = new GetReadmeItemsQuery();
      var readmeItems = await _mediator.Send(readmeQueryItems);

      // Get all projects
      var projectsQuery = new GetAllProjectsQuery();
      var projects = await _mediator.Send(projectsQuery);

      // Combine into comprehensive response
      return new ReadmeResponse {
        Realm = Cx.AppName,
        Version = Cx.AppVersion,
        ActiveProjects = projects,
        AdditionalDetails = readmeItems
      };

    }
  }

  public record ReadmeResponse { 
    public string Realm { get; init; } = string.Empty;
    public string Version { get; init; } = string.Empty;    
    public List<ProjectDto> ActiveProjects { get; init; } = new();
    public List<ReadmeItemDto> AdditionalDetails { get; init; } = new();

  }
}
