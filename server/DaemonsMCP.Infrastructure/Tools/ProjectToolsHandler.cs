using DaemonsMCP.Application.Projects.Queries.GetAllProjects;
using DaemonsMCP.Domain.Constants;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Infrastructure.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DaemonsMCP.Infrastructure.Tools {
  public class ProjectToolsHandler : IProjectToolsHandler {
    private ILogger<ProjectToolsHandler> _logger;
    private IMediator _mediator;

    public ProjectToolsHandler(
      ILogger<ProjectToolsHandler> logger,
      IMediator mediator
    ) {
      _logger = logger;
      _mediator = mediator;
    }

    public async Task<string> ListProjectsAsync() {
      try { 
        var query = new GetAllProjectsQuery();        
        var projects = await _mediator.Send(query);
        var opResult = McpOpResult.CreateSuccess(Cx.ListProjectsCmd, $"{Cx.ListProjectsCmd} Success.", projects.ToArray());
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error listing projects");
        var opResult = McpOpResult.CreateFailure(Cx.ListProjectsCmd, $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }
  }

  public interface IProjectToolsHandler {
    public Task<string> ListProjectsAsync();
  }
}
