using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Infrastructure.Services;
using MediatR;
using DaemonsMCP.Application.FileSystem.Commands.SyncProjectFileSystem;



namespace DaemonsMCP.Api.Extensions {
  public static class IndexingEndpointExt {

    public static WebApplication MapIndexingEndpoints(this WebApplication app) {
      
      app.MapPost("/api/indexing/sync",
        async (int? projectId, IIndexingService indexingService) => {
          try {
            var result = await indexingService.RunAsync(projectId);
            return Results.Ok(new {
              result.Success,
              result.FilesProcessed,
              result.FilesFailed,
              DurationSeconds = result.Duration.TotalSeconds,
              result.ErrorMessage
            });
          } catch (Exception) {
            return Results.Problem("An error occurred while processing the request.");
          }
        })
      .WithName("RunIndexing")
      .WithDescription("Runs the indexing process for items in the IndexQueue.");

      app.MapGet("/api/indexing/queue/status",
        async (int? projectId, IIndexQueueRepository queueRepo) => {
          var pending = await queueRepo.GetPendingAsync(projectId, batchSize: 1000);
          var pendingCount = pending.Count();

          return Results.Ok(new {
            PendingCount = pendingCount,
            ProjectId = projectId
          });
        })
      .WithName("GetIndexingQueueStatus")
      .WithDescription("Gets the status of the indexing queue, including the number of pending items.");

      return app;
    }

  }
}
