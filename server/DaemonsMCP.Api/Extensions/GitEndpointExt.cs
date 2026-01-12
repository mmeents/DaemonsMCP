using DaemonsMCP.Application.Git.Commands.ScanProjectForGitRepos;
using DaemonsMCP.Application.Git.Queries.GetGitBranchesByRepo;
using DaemonsMCP.Application.Git.Commands.ScanRepoForGitBranches;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using DaemonsMCP.Application.Git.Queries.GetGitReposForProject;
using System.ComponentModel;

namespace DaemonsMCP.Api.Extensions {
  public static class GitEndpointExt {
    public static WebApplication MapGitEndpoints(this WebApplication app) {
            
      app.MapGet("/api/git/branches", async (
        [FromQuery] int gitRepositoryId,
        IMediator mediator
      ) => {
        var command = new GetGitBranchesByRepoQuery(gitRepositoryId);
        var result = await mediator.Send(command);
        return result;
      });

      app.MapGet("/api/git/branches/scan", async (
         [FromQuery] int gitRepositoryId,
         IMediator mediator
       ) => {
         var command = new ScanRepoForGitBranchesCommand(gitRepositoryId);
         var result = await mediator.Send(command);
         return result;
       });

      app.MapGet("/api/git/repositories", async (
        [FromQuery] int projectId,
        IMediator mediator
      ) => {
        var command = new GetGitReposForProjectCommand(projectId);
        var result = await mediator.Send(command);
        return result;
      });

      app.MapGet("/api/git/repositories/scan", async (
        [FromQuery] int projectId,
        IMediator mediator
      ) => {
        var command = new ScanProjectForGitReposCommand(projectId);
        var result = await mediator.Send(command);
        return result;
      });


      return app;
    }
  }
}
