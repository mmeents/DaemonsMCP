using Azure;
using DaemonsMCP.Application.FileSystem.Queries.SearchFileSystem;
using DaemonsMCP.Application.ObjectHierarchy.Queries.SearchObjectHierarchy;
using DaemonsMCP.Domain.Constants;
using DaemonsMCP.Domain.Models;
using MediatR;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;



namespace DaemonsMCP.Infrastructure.Tools {
  public class ObjectHierarchyToolsHandler : IObjectHierarchyToolsHandler {
    private ILogger<ObjectHierarchyToolsHandler> _logger;
    private IServiceScopeFactory _scopeFactory;

    public ObjectHierarchyToolsHandler(
      ILogger<ObjectHierarchyToolsHandler> logger,
      IServiceScopeFactory scopeFactory
    ) {
      _logger = logger;
      _scopeFactory = scopeFactory;
    }

    public async Task<string> SearchObjectHierarchy(SearchObjectHierarchyQuery request) {
      try {
        // Create a scope to resolve scoped services like IMediator and repositories
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
               
        var result = await mediator.Send(request);
        var opResult = McpOpResult.CreateSuccess(Cx.SearchObjectHierarchyCmd, $"{Cx.SearchObjectHierarchyCmd} Success.", result);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error Searching Object Hierarchy");
        var opResult = McpOpResult.CreateFailure(Cx.SearchObjectHierarchyCmd, $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }
  }


  public interface IObjectHierarchyToolsHandler {
    Task<string> SearchObjectHierarchy(SearchObjectHierarchyQuery request);

  }



}
