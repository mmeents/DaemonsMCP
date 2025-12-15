using DaemonsMCP.Application.AccessTokens.Commands.CreateAccessToken;
using DaemonsMCP.Application.AccessTokens.Commands.RevokeAccessToken;
using DaemonsMCP.Application.AccessTokens.Queries.SearchAccessTokens;
using DaemonsMCP.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Daemons.Web.Extensions {
  public static class AccessTokenEndpointExt {
    public static WebApplication MapAccessTokenEndpoints(this WebApplication app) {

      // Search access tokens (paginated)
      app.MapGet("/api/tokens/search", async (
        IMediator mediator,
        [FromQuery] string? issuedTo = null,
        [FromQuery] bool includeExpired = false,
        [FromQuery] bool includeUsed = false,
        [FromQuery] int pageNo = 1,
        [FromQuery] int pageSize = 20) => {

          var query = new SearchAccessTokensQuery(
            issuedTo,
            includeExpired,
            includeUsed,
            pageNo,
            pageSize);

          var result = await mediator.Send(query);
          return Results.Ok(new { success = true, data = result });
        }).WithName("SearchAccessTokens");

      // Get access token by ID
      app.MapGet("/api/tokens/{id}", async (
        int id,
        IMediator mediator) => {

          var query = new GetAccessTokenByIdQuery(id);
          var token = await mediator.Send(query);

          return token is not null
            ? Results.Ok(new { success = true, data = token })
            : Results.NotFound(new { success = false, errorMessage = "Token not found" });
        }).WithName("GetAccessTokenById");

      // Create a new access token
      app.MapPost("/api/tokens", async (
        CreateAccessTokenCommand command,
        IMediator mediator) => {

          var result = await mediator.Send(command);
          return Results.Created($"/api/tokens/{result.Id}", new { success = true, data = result });
        }).WithName("CreateAccessToken");

      // Revoke access token (and its chain)
      app.MapPost("/api/tokens/{id}/revoke", async (
        int id,
        IMediator mediator) => {

          var command = new RevokeAccessTokenCommand(id);
          var success = await mediator.Send(command);

          return success
            ? Results.Ok(new { success = true, data = (object?)null })
            : Results.NotFound(new { success = false, errorMessage = "Token not found" });
        }).WithName("RevokeAccessToken");

      return app;
    }
  }
}
