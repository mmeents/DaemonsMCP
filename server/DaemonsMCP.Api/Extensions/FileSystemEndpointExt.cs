using DaemonsMCP.Application.FileSystem.Commands.SyncProjectFileSystem;
using DaemonsMCP.Application.FileSystem.Queries.SearchFileSystem;
using DaemonsMCP.Application.FileSystem.Commands.CreateProjectFile;
using DaemonsMCP.Application.FileSystem.Commands.CreateProjectFolder;
using DaemonsMCP.Application.FileSystem.Commands.UpdateProjectFile;
using MediatR;
using DaemonsMCP.Application.FileSystem.Queries.GetFileContents;

namespace DaemonsMCP.Api.Extensions {
  public static class FileSystemEndpointExt {
    public static WebApplication MapFileSystemEndpoints(this WebApplication app) {

      app.MapPost("/api/filesystem/sync/{projectId}",
       async (int projectId, IMediator mediator) => {
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
       }).WithDisplayName("SyncFileSystem")
         .WithDescription("Synchronizes the files in the file system by project with db table which search depends on.");

      // Search file system 
      app.MapGet("/api/filesystem/search", async (
        IMediator mediator,
        int projectId,
        string? filter = null,
        bool includeDirectories = true,
        bool includeFiles = true,
        int pageNo = 1,
        int pageSize = 20) => {
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
      .WithName("SearchFileSystem")
      .WithDescription("Searches the file system for items matching the specified criteria.");

      // Get file system Item by project ID
      app.MapGet("/api/filesystem/{projectId}", async (
          IMediator mediator,
          int projectId,
          int fileSystemNodeId) => {
            var query = new GetFileContentsQuery(projectId, fileSystemNodeId);
            var nodes = await mediator.Send(query);
            return Results.Ok(nodes);
          });

      app.MapPost("/api/filesystem/createfile/{projectId}", async (
          int projectId,
          string relativePath,
          string content,
          IMediator mediator) => {
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
          IMediator mediator) => {
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
          IMediator mediator) => {
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

      return app;
    }

  }
}
