using DaemonsMCP.Application.Projects.Queries.GetAllProjects;
using DaemonsMCP.Application.Items.Queries.GetReadmeItems;
using DaemonsMCP.Domain.Constants;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DaemonsMCP.Application.ForWeb.GetReadme {
  public class GetReadmeForWebQueryHandler : IRequestHandler<GetReadmeForWebQuery, ReadmeWebResponse> {
    private readonly IAccessTokenRepository _tokenRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IMediator _mediator;
    private readonly ILogger<GetReadmeForWebQueryHandler> _logger;

    public GetReadmeForWebQueryHandler(
        IAccessTokenRepository tokenRepository,
        IItemRepository itemRepository,
        IProjectRepository projectRepository,
        IMediator mediator, 
        ILogger<GetReadmeForWebQueryHandler> logger) {
      _tokenRepository = tokenRepository;
      _itemRepository = itemRepository;
      _projectRepository = projectRepository;
      _mediator = mediator;
      _logger = logger;
    }

    public async Task<ReadmeWebResponse> Handle(GetReadmeForWebQuery request, CancellationToken cancellationToken) {

      // 1. Get README content from Items system via mcp route
      var readmequery = new GetReadmeItemsQuery();
      var readmeDtos = await _mediator.Send(readmequery, cancellationToken);
      
      var readmeContent = new ReadmeContentDto {        
        Content = readmeDtos,
        Version = Cx.AppVersion,
        LastUpdated = readmeDtos.Max(r => r.Modified)
      };

      // 2. Get endpoints (static registry)
      var endpoints = EndpointRegistry.GetAll();

      // 3. Get projects (live data)
      var projects = await _projectRepository.GetAllAsync(cancellationToken);
      var projectDtos = projects
        .Select(p => new ProjectDto(p.Id, p.Name, p.Description, p.RootPath, p.CreatedAt))
        .ToList();     
      _logger.LogInformation("Handled Readme detail for {ProjectCount} projects", projectDtos.Count);

      // 4. Return everything
      return new ReadmeWebResponse {
        AccessAndAuthorizationDetails = "You used a one-time access token to retrieve this response. For your next request, extract the 'nextToken' value from this output—it will serve as your new access token. Tokens are single-use only, expire after a maximum of 4 hours from issuance, and are automatically destroyed upon validation.",
        Readme = readmeContent,
        Endpoints = endpoints,
        Projects = projectDtos        
      };
    }
  }
}
