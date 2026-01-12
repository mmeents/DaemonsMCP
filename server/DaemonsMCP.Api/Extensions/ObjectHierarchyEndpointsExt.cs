using DaemonsMCP.Application.FileSystem.Commands.SyncProjectFileSystem;
using DaemonsMCP.Application.FileSystem.Queries.SearchFileSystem;
using DaemonsMCP.Application.FileSystem.Commands.CreateProjectFile;
using DaemonsMCP.Application.FileSystem.Commands.CreateProjectFolder;
using DaemonsMCP.Application.FileSystem.Commands.UpdateProjectFile;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Application.Projects.Commands.CreateProject;
using DaemonsMCP.Application.Projects.Queries.GetAllProjects;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using DaemonsMCP.Application.FileSystem.Queries.GetFileContents;

namespace DaemonsMCP.Api.Extensions {

  public static class ObjectHierarchyEndpointsExt {
    
    public static WebApplication MapObjectHierarchyEndpoints(this WebApplication app) {

      app.MapGet("/api/hierarchy/search", async (
        IMediator mediator,
        int projectId,
        string? searchTerm = null,
        int? identifierTypeId = null,
        int? fileSystemNodeId = null,
        int? parentId = null,
        int pageNo = 1,
        int pageSize = 20) =>
        {
          var query = new SearchObjectHierarchyQuery(
              projectId,
              searchTerm,
              identifierTypeId,
              fileSystemNodeId,
              parentId,
              pageNo,
              pageSize);

          var result = await mediator.Send(query);
          return Results.Ok(result);
        })
      .WithName("SearchObjectHierarchy")
      .WithDescription("Searches the object hierarchy for items matching the specified criteria.");


      return app;
    }


  }

}
