using DaemonsMCP.Application.Invitations.Queries.SearchInvitationTokens;
using DaemonsMCP.Application.Invitations.Commands.CreateInvitationToken;
using DaemonsMCP.Application.Invitations.Queries.GetInvitationTokenById;
using DaemonsMCP.Application.Invitations.Commands.RegisterWithInvitation;
using DaemonsMCP.Application.Invitations.Commands.RevokeInvitation;
using DaemonsMCP.Application.Invitations.Queries.ValidateInvitation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DaemonsMCP.Api.Extensions {
  public static class InvitationTokenEndpointExt {
    public static WebApplication MapInvitationTokenEndpoints(this WebApplication app) {

      app.MapGet("/api/invitations/validate", async (
      [FromQuery] string token,
      IMediator mediator) => {
        var query = new ValidateInvitationTokenQuery(token);
        var result = await mediator.Send(query);
        return result is not null
          ? Results.Ok(new { success = true, data = result })
          : Results.BadRequest(new { success = false, errorMessage = "Invalid or expired invitation token" });
      }).WithName("ValidateInvitationToken");

      app.MapGet("/api/invitations/search", async (
        IMediator mediator,
        [FromQuery] string? invitedEmail = null,
        [FromQuery] bool includeExpired = false,
        [FromQuery] bool includeUsed = false,
        [FromQuery] int pageNo = 1,
        [FromQuery] int pageSize = 20) => {

        var query = new SearchInvitationTokensQuery(
          invitedEmail,
          includeExpired,
          includeUsed,
          pageNo,
          pageSize);

        var result = await mediator.Send(query);
        return Results.Ok(new { success = true, data = result });
      }).WithName("SearchInvitationTokens");

      app.MapGet("/api/invitations/{id}", async (
        int id,
        IMediator mediator) => {

        var query = new GetInvitationTokenByIdQuery(id);
        var token = await mediator.Send(query);

        return token is not null
          ? Results.Ok(new { success = true, data = token })
          : Results.NotFound(new { success = false, errorMessage = "Invitation token not found" });
      }).WithName("GetInvitationTokenById");


      app.MapPost("/api/invitations", async (
        IMediator mediator,
        [FromBody] CreateInvitationCommand command) => {
        var result = await mediator.Send(command);
        return Results.Ok(new { success = true, data = result });
      }).WithName("CreateInvitationToken");

      app.MapPost("/api/invitations/register", async (
        IMediator mediator,
        [FromBody] RegisterWithInvitationCommand command) => {
        try {
          var result = await mediator.Send(command);
          return Results.Ok(new { success = true, data = result });
        } catch (UnauthorizedAccessException ex) {
          return Results.Unauthorized();
        } catch (Exception ex) {
          return Results.BadRequest(new { success = false, errorMessage = "Error occured." });
        }
      }).WithName("RegisterWithInvitationToken");

      app.MapPost("/api/invitations/{id}/revoke", async (
        int id,
        IMediator mediator) => {
        try {
          var result = await mediator.Send(new RevokeInvitationCommand(id));
          return Results.Ok(new { success = true, data = result });
        } catch (UnauthorizedAccessException ex) {
          return Results.Unauthorized();
        } catch (Exception ex) {
          return Results.BadRequest(new { success = false, errorMessage = "Error occured." });
        }
      }).WithName("RevokeInvitationToken");


      return app;
    }
  }
}
