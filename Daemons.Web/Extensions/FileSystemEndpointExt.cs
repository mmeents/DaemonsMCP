using DaemonsMCP.Application.AccessTokens.Commands.MarkTokenUsed;
using DaemonsMCP.Application.AccessTokens.Commands.ValidateToken;
using DaemonsMCP.Application.FileSystem.Queries.GetFileContents;
using DaemonsMCP.Application.FileSystem.Queries.SearchFileSystem;
using DaemonsMCP.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace Daemons.Web.Extensions {

  
  public static class FileSystemEndpointExt {
    public static WebApplication MapFileSystemEndpoints(this WebApplication app) {

      app.MapGet("/api/files/search", async (
        IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken,
        [FromQuery] string token,
        [FromQuery] int projectId,
        [FromQuery] string? filter = null,
        [FromQuery] bool includeDirectories = true,
        [FromQuery] bool includeFiles = true,
        [FromQuery] int pageNo = 1,
        int pageSize = 20) => {

          AccessTokenDto? nextToken = null;
          try {
            var thisUrlUsed = $"GET /api/files/search?token={token}&projectId={projectId}&filter={filter}&" +
                $"includeDirectories={includeDirectories}&includeFiles={includeFiles}&pageNo={pageNo}&pageSize={pageSize}";

            var thisToken = await mediator.Send(new ValidateTokenCommand(token, thisUrlUsed), cancellationToken);

            var query = new SearchFileSystemQuery(
              projectId,
              filter,
              includeDirectories,
              includeFiles,
              pageNo,
              pageSize);
            var result = await mediator.Send(query);

            if (thisToken.UsedUrl == null) { 
              var usedBy = context.Connection.RemoteIpAddress?.ToString() ?? "RemoteIp was empty";
              nextToken = await mediator.Send(
                new MarkTokenUsedCommand(token, thisUrlUsed, usedBy),
                cancellationToken);
            }

            WebOpResult r2 = WebOpResult.CreateSuccess(
              "SearchFileSystem",
              $"Search completed for project {projectId} with filter '{filter}'",
              nextToken.Token,
              result);

            return Results.Ok(r2.ToString());

          } catch (UnauthorizedAccessException ex) {
            var result = WebOpResult.CreateFailure("SearchFileSystem", token, "Invalid access token.", ex);
            return Results.Problem(result.ToString());
          } catch (Exception ex) {
            var result = WebOpResult.CreateFailure("SearchFileSystem", nextToken?.Token ?? "", "Error searching file system.", ex);
            return Results.Problem(result.ToString());
          }
        }).WithName("SearchFileSystem");


      // Get file system Item by project ID
      app.MapGet("/api/files/get", async (
        IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken,
        [FromQuery] string token,
        [FromQuery] int projectId,
        [FromQuery] int fileSystemNodeId) => {

        AccessTokenDto? nextToken = null;
        try {
          var thisUrlUsed = $"GET /api/files/get?token={token}&projectId={projectId}&fileSystemNodeId={fileSystemNodeId}";
          var thisToken = await mediator.Send(new ValidateTokenCommand(token, thisUrlUsed), cancellationToken);

          var query = new GetFileContentsQuery(projectId, fileSystemNodeId);
          var nodes = await mediator.Send(query);

          var usedBy = context.Connection.RemoteIpAddress?.ToString() ?? "RemoteIp was empty";
          nextToken = await mediator.Send(
            new MarkTokenUsedCommand(token, thisUrlUsed, usedBy),
            cancellationToken);

          WebOpResult r2 = WebOpResult.CreateSuccess(
            "GetFileContents",
            $"Retrieved file contents for project {projectId}, Id: {fileSystemNodeId}",
            nextToken.Token,
            nodes);
            return Results.Ok(r2);

        } catch (UnauthorizedAccessException ex) {
            var result = WebOpResult.CreateFailure("SearchFileSystem", token, "Invalid access token.", ex);
            return Results.Problem(result.ToString());
        } catch (Exception ex) {
          var result = WebOpResult.CreateFailure("GetFile", token, "Error getting file.", ex);
          return Results.Problem(result.ToString());
        }
      });

      return app;
    }
  }
}
