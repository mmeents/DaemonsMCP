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

  public static class EndpointExtensions {
    public static WebApplication MapProjectEndpoints(this WebApplication app) {     

      app.MapGet("/api/readme", async (IMediator mediator) =>
      {
        var query = new GetReadmeQuery();
        var results = await mediator.Send(query);
        return Results.Ok(results);
      });

      // Get all projects
      app.MapGet("/api/projects", async (IMediator mediator) =>
      {
        var query = new GetAllProjectsQuery();
        var results = await mediator.Send(query);
        return Results.Ok(results);
      });

      // Get project by ID
      app.MapGet("/api/projects/{id}", async (
          int id,
          IMediator mediator,
          IProjectRepository repo) =>
      {
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

    public static WebApplication MapFileSystemEndpoints(this WebApplication app) {

      // Search file system 
      app.MapGet("/api/filesystem/search", async (
        IMediator mediator,
        int projectId,
        string? filter = null,
        bool includeDirectories = true,
        bool includeFiles = true,
        int pageNo = 1,
        int pageSize = 20) =>
      {
        var query = new SearchFileSystemQuery(
            projectId,
            filter,
            includeDirectories,
            includeFiles,
            pageNo,
            pageSize);

        var result = await mediator.Send(query);
        return Results.Ok(result);
      })
      .WithName("SearchFileSystem");

      // Get file system Item by project ID
      app.MapGet("/api/filesystem/{projectId}", async (
          IMediator mediator,
          int projectId,
          int fileSystemNodeId) =>
      {
        var query = new GetFileContentsQuery(projectId, fileSystemNodeId);
        var nodes = await mediator.Send(query);
        return Results.Ok(nodes);
      });

      app.MapPost("/api/filesystem/createfile/{projectId}", async (
          int projectId,
          string relativePath,
          string content,
          IMediator mediator) =>
      {
        try {
          var command = new CreateProjectFileCommand(projectId, relativePath, content);
          var result = await mediator.Send(command);
          return Results.Ok(new {
            Success = true,
            ProjectId = projectId,
            Result = result
          });
        } catch (Exception ex) {
          return Results.BadRequest(new {
            Success = false,
            Error = ex.Message
          });
        }
      });

      app.MapPost("/api/filesystem/createfolder/{projectId}", async (
          int projectId,
          string relativePath,
          IMediator mediator) =>
      {
          try {
          var command = new CreateProjectFolderCommand(projectId, relativePath);
          var result = await mediator.Send(command);
          return Results.Ok(new {
              Success = true,
              ProjectId = projectId,
              Result = result
          });
          } catch (Exception ex) {
          return Results.BadRequest(new {
              Success = false,
              Error = ex.Message
          });
          }
      });

      app.MapPost("/api/filesystem/updatefile/{projectId}", async (
          int projectId,
          int fileSystemNodeId,
          string content,
          IMediator mediator) =>
      {
        try {
          var command = new UpdateProjectFileCommand(projectId, fileSystemNodeId, content);
          var result = await mediator.Send(command);
          return Results.Ok(new {
            Success = true,
            ProjectId = projectId,
            Result = result
          });
        } catch (Exception ex) {
          return Results.BadRequest(new {
            Success = false,
            Error = ex.Message
          });
        }
      });


      app.MapPost("/api/filesystem/sync/{projectId}", async (
          int projectId,
          IMediator mediator) =>
      {
        try {
          var command = new SyncProjectFileSystemCommand(projectId);
          var result = await mediator.Send(command);
          return Results.Ok(new {
            Success = true,
            ProjectId = projectId,
            Result = result
          });
        } catch (Exception ex) {
          return Results.BadRequest(new {
            Success = false,
            Error = ex.Message
          });
        }
      });

      


      return app;
    }

    public static WebApplication MapIndexingEndpoints(this WebApplication app) {
      app.MapPost("/api/indexing/run", async (
          int? projectId,
          IIndexingService indexingService) =>
      {
        try {
          var result = await indexingService.RunAsync(projectId);
          return Results.Ok(new {
            Success = result.Success,
            FilesProcessed = result.FilesProcessed,
            FilesFailed = result.FilesFailed,
            DurationSeconds = result.Duration.TotalSeconds,
            ErrorMessage = result.ErrorMessage
          });
        } catch (Exception ex) {
          return Results.Problem(ex.Message);
        }
      })
      .WithName("RunIndexing");

      app.MapGet("/api/indexing/queue/status", async (
          int? projectId,
          IIndexQueueRepository queueRepo) =>
      {
        var pending = await queueRepo.GetPendingAsync(projectId, batchSize: 1000);
        var pendingCount = pending.Count();

        return Results.Ok(new {
          PendingCount = pendingCount,
          ProjectId = projectId
        });
      })
      .WithName("GetIndexingQueueStatus");

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
      .WithName("SearchObjectHierarchy");


      return app;
    }
  }

}
