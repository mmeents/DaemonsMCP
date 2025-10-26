using DaemonsMCP.Application.Projects.Queries.GetAllProjects;
using DaemonsMCP.Domain.Constants;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Infrastructure.Extensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
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
    private IServiceScopeFactory _scopeFactory;

    public ProjectToolsHandler(
      ILogger<ProjectToolsHandler> logger,
      IServiceScopeFactory scopeFactory
    ) {
      _logger = logger;
      _scopeFactory = scopeFactory;
    }

    public async Task<string> ListProjectsAsync() {
      try {
        // Create a scope to resolve scoped services like IMediator and repositories
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        var query = new GetAllProjectsQuery();        
        var projects = await mediator.Send(query);
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
