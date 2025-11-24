using DaemonsMCP.Application.Items.Commands.AddUpdateItem;
using DaemonsMCP.Application.Items.Commands.DeleteItem;
using DaemonsMCP.Domain.Enums;
using DaemonsMCP.Domain.Models;
using MediatR;

namespace DaemonsMCP.Api.Extensions {
  public static class ItemsEndpointExt {

    public static WebApplication MapItemsEndpoints(this WebApplication app) {
      // Search items with optional filters and depth
      app.MapGet("/api/items/search", async (
        IMediator mediator,
        int? parentId = null,
        string? nameContains = null,
        string? detailsContains = null,
        int? typeId = null,
        int? statusId = null,
        int maxDepth = 1) => {
          try {
            var query = new SearchItemsQuery(
              parentId,
              nameContains,
              detailsContains,
              typeId,
              statusId,
              maxDepth);

            var result = await mediator.Send(query);
            return Results.Ok(result);
          } catch (Exception ex) {
            return Results.BadRequest(new {
              Success = false,
              Error = ex.Message
            });
          }
        })
      .WithName("SearchItems");

      // Get item by ID with optional depth
      app.MapGet("/api/items/{itemId}", async (
        IMediator mediator,
        int itemId,
        int maxDepth = 1) => {
          try {
            var query = new GetItemByIdQuery(itemId, maxDepth);
            var result = await mediator.Send(query);

            if (result == null) {
              return Results.NotFound(new {
                Success = false,
                Error = $"Item with ID {itemId} not found"
              });
            }

            return Results.Ok(result);
          } catch (Exception ex) {
            return Results.BadRequest(new {
              Success = false,
              Error = ex.Message
            });
          }
        })
      .WithName("GetItemById");

      // Add or update item
      app.MapPost("/api/items", async (
        IMediator mediator,
        AddUpdateItemRequest request) => {
          try {
            var command = new AddUpdateItemCommand(
              request.Id,
              request.ParentId,
              request.ItemTypeId,
              request.StatusTypeId,
              request.Rank,
              request.Name,
              request.Details,
              request.ReferenceFileSystemId,
              request.ReferenceObjectHierarchyId);

            var result = await mediator.Send(command);
            return Results.Ok(result);
          } catch (Exception ex) {
            return Results.BadRequest(new {
              Success = false,
              Error = ex.Message
            });
          }
        })
      .WithName("AddUpdateItem");

      app.MapDelete("/api/items/{itemId}", async (
        IMediator mediator,
        int itemId,
         DeleteStrategy strategy = DeleteStrategy.DeleteCascade) => {
          try {
            var command = new DeleteItemCommand(itemId, strategy);
            var result = await mediator.Send(command);
            return Results.Ok(result);
          } catch (Exception ex) {
            return Results.BadRequest(new {
              Success = false,
              Error = ex.Message
            });
          }
        });

      // Get all item types
      app.MapGet("/api/items/types/all", async (
        IMediator mediator) => {
          try {
            var query = new GetAllItemTypesQuery();
            var result = await mediator.Send(query);
            return Results.Ok(result);
          } catch (Exception ex) {
            return Results.BadRequest(new {
              Success = false,
              Error = ex.Message
            });
          }
        })
      .WithName("GetAllItemTypes");

      // Get item types (non-status types)
      app.MapGet("/api/items/types", async (
        IMediator mediator) => {
          try {
            var query = new GetItemTypesQuery();
            var result = await mediator.Send(query);
            return Results.Ok(result);
          } catch (Exception ex) {
            return Results.BadRequest(new {
              Success = false,
              Error = ex.Message
            });
          }
        })
      .WithName("GetItemTypes");

      // Get status types
      app.MapGet("/api/items/status-types", async (
        IMediator mediator) => {
          try {
            var query = new GetStatusTypesQuery();
            var result = await mediator.Send(query);
            return Results.Ok(result);
          } catch (Exception ex) {
            return Results.BadRequest(new {
              Success = false,
              Error = ex.Message
            });
          }
        })
      .WithName("GetStatusTypes");

      // Add or update item type
      app.MapPost("/api/items/types", async (
        IMediator mediator,
        AddUpdateItemTypeRequest request) => {
          try {
            var command = new AddUpdateItemTypeCommand(
              request.Id,
              request.Name,
              request.Description,
              request.Rank,
              request.ParentId);

            var result = await mediator.Send(command);
            return Results.Ok(result);
          } catch (Exception ex) {
            return Results.BadRequest(new {
              Success = false,
              Error = ex.Message
            });
          }
        })
      .WithName("AddUpdateItemType");

      return app;
    }

  }
}
