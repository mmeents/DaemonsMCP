using DaemonsMCP.Application.Projects.Commands.CreateProject;
using DaemonsMCP.Application.Projects.Queries.GetAllProjects;
using DaemonsMCP.Application.Readme.Queries.GetReadme;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Domain.Models;
using MediatR;

namespace DaemonsMCP.Api.Extensions {
  public static class ProjectEndpointExt {
    public static WebApplication MapProjectEndpoints(this WebApplication app) {

      app.MapGet("/api/readme", async (IMediator mediator) => {
        var query = new GetReadmeQuery();
        var results = await mediator.Send(query);
        return Results.Ok(results);
      });

      // Get all projects
      app.MapGet("/api/projects", async (IMediator mediator) => {
        var query = new GetAllProjectsQuery();
        var results = await mediator.Send(query);
        return Results.Ok(results);
      });

      // Get project by ID
      app.MapGet("/api/projects/{id}", async (
          int id,
          IMediator mediator,
          IProjectRepository repo) => {
            var project = await repo.GetByIdAsync(id);
            return project is not null ? Results.Ok(project) : Results.NotFound();
          });

      // Create a new project
      app.MapPost("/api/projects", async (
       CreateProjectCommand command,
       IMediator mediator) => {
         var result = await mediator.Send(command);
         return Results.Created($"/api/projects/{result.Id}", result);
       });

      return app;
    }
  }
}
