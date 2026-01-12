using DaemonsMCP.Application.Models.Commands.AddUpdateModel;
using DaemonsMCP.Application.Models.Commands.DeleteModel;
using DaemonsMCP.Application.Models.Queries.GetModelById;
using DaemonsMCP.Application.Models.Queries.GetModelProperties;
using DaemonsMCP.Application.Models.Queries.SearchModels;
using DaemonsMCP.Application.ModelTypes.Queries.GetAllModelTypes;
using DaemonsMCP.Application.ModelTypes.Queries.GetEditorTypes;
using DaemonsMCP.Application.ModelTypes.Queries.GetProjectTemplates;
using DaemonsMCP.Application.ModelTypes.Queries.GetSqlDataTypes;
using DaemonsMCP.Application.ModelTypes.Queries.GetValidChildTypes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DaemonsMCP.Api.Extensions {
  public static class ModelsEndpointExt {
    public static WebApplication MapModelsEndpoints(this WebApplication app) {

      // Search models
      app.MapGet("/api/models/search", async (
        IMediator mediator,
        [FromQuery] int projectId,
        [FromQuery] int? parentId = null,
        [FromQuery] int? modelTypeId = null,
        [FromQuery] string? nameFilter = null,
        [FromQuery] int maxDepth = 1,
        [FromQuery] bool includeProperties = false) => {
          try {
            var query = new SearchModelsQuery(
              projectId,
              parentId,
              modelTypeId,
              nameFilter,
              maxDepth,
              includeProperties);

            var result = await mediator.Send(query);
            return Results.Ok(result);
          } catch (Exception ex) {
            return Results.BadRequest(new {
              Success = false,
              Error = ex.Message
            });
          }
        })
      .WithName("SearchModels");

      // Get model by ID
      app.MapGet("/api/models/{modelId}", async (
        IMediator mediator,
        int modelId,
        int maxDepth = 1,
        bool includeProperties = true) => {
          try {
            var query = new GetModelByIdQuery(modelId, maxDepth, includeProperties);
            var result = await mediator.Send(query);

            if (result == null) {
              return Results.NotFound(new {
                Success = false,
                Error = $"Model with ID {modelId} not found"
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
      .WithName("GetModelById");

      // Add or update model
      app.MapPost("/api/models", async (
        IMediator mediator,
        AddUpdateModelCommand command) => {
          try {
            var result = await mediator.Send(command);
            return Results.Ok(result);
          } catch (Exception ex) {
            return Results.BadRequest(new {
              Success = false,
              Error = ex.Message
            });
          }
        })
      .WithName("AddUpdateModel");

      // Delete model
      app.MapDelete("/api/models/{modelId}", async (
        IMediator mediator,
        int modelId) => {
          try {
            var command = new DeleteModelCommand(modelId);
            var result = await mediator.Send(command);
            return Results.Ok(new { Success = result });
          } catch (Exception ex) {
            return Results.BadRequest(new {
              Success = false,
              Error = ex.Message
            });
          }
        })
      .WithName("DeleteModel");

      // Get model properties
      app.MapGet("/api/models/{modelId}/properties", async (
        IMediator mediator,
        int modelId) => {
          try {
            var query = new GetModelPropertiesQuery(modelId);
            var result = await mediator.Send(query);
            return Results.Ok(result);
          } catch (Exception ex) {
            return Results.BadRequest(new {
              Success = false,
              Error = ex.Message
            });
          }
        })
      .WithName("GetModelProperties"); 

      // Add or update model property
      app.MapPost("/api/models/properties", async (
        IMediator mediator,
        AddUpdateModelPropertyCommand command) => {
          try {
            var result = await mediator.Send(command);
            return Results.Ok(result);
          } catch (Exception ex) {
            return Results.BadRequest(new {
              Success = false,
              Error = ex.Message
            });
          }
        })
      .WithName("AddUpdateModelProperty");

      app.MapDelete("/api/models/properties/{propertyId}", async (
        IMediator mediator,
        int propertyId) => {
          try {
            var command = new DeleteModelPropertyCommand(propertyId);
            var result = await mediator.Send(command);
            return Results.Ok(new { Success = result });
          } catch (Exception ex) {
            return Results.BadRequest(new {
              Success = false,
              Error = ex.Message
            });
          }
        })
      .WithName("DeleteModelProperty");

      // ModelTypes endpoints
      app.MapGet("/api/modeltypes/all", async (
        IMediator mediator) => {
          try {
            var query = new GetAllModelTypesQuery();
            var result = await mediator.Send(query);
            return Results.Ok(result);
          } catch (Exception ex) {
            return Results.BadRequest(new {
              Success = false,
              Error = ex.Message
            });
          }
        })
      .WithName("GetAllModelTypes");

      
      app.MapGet("/api/modeltypes/editors", async (
        IMediator mediator) => {
          try {
            var query = new GetEditorTypesQuery();
            var result = await mediator.Send(query);
            return Results.Ok(result);
          } catch (Exception ex) {
            return Results.BadRequest(new {
              Success = false,
              Error = ex.Message
            });
          }
        })
      .WithName("GetEditorTypes");

      app.MapGet("/api/modeltypes/sqltypes", async (
        IMediator mediator) => {
          try {
            var query = new GetSqlDataTypesQuery();
            var result = await mediator.Send(query);
            return Results.Ok(result);
          } catch (Exception ex) {
            return Results.BadRequest(new {
              Success = false,
              Error = ex.Message
            });
          }
        })
      .WithName("GetSqlDataTypes");

      app.MapGet("/api/modeltypes/templates", async (
        IMediator mediator) => {
          try {
            var query = new GetProjectTemplatesQuery();
            var result = await mediator.Send(query);
            return Results.Ok(result);
          } catch (Exception ex) {
            return Results.BadRequest(new {
              Success = false,
              Error = ex.Message
            });
          }
        })
      .WithName("GetProjectTemplates");

      app.MapGet("/api/modeltypes/{parentModelTypeId}/valid-children", async (
        IMediator mediator,
        int parentModelTypeId) => {
          try {
            var query = new GetValidChildTypesQuery(parentModelTypeId);
            var result = await mediator.Send(query);
            return Results.Ok(result);
          } catch (Exception ex) {
            return Results.BadRequest(new {
              Success = false,
              Error = ex.Message
            });
          }
        })
      .WithName("GetValidChildTypes");


      return app;
    }

  }
}
