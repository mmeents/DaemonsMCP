using DaemonsMCP.Application.AccessTokens.Commands.MarkTokenUsed;
using DaemonsMCP.Application.AccessTokens.Commands.ValidateToken;
using DaemonsMCP.Application.ForWeb.GetReadme;
using DaemonsMCP.Domain.Extensions;
using DaemonsMCP.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Daemons.Web.Extensions {
  public record ReadmeRequest(
    string Token);
  public static class ProjectEndpointExt {
    public static WebApplication MapProjectEndpoints(this WebApplication app) {

      app.MapGet("/api/readme", async (        
        HttpContext context,
        IMediator mediator,
        CancellationToken cancellationToken,
        [FromQuery] string token ) =>
      {
        AccessTokenDto? nextToken = null;
        try {                   
          var thisUrlUsed = $"GET /api/readme?token={token}";
          var thisToken = await mediator.Send(new ValidateTokenCommand(token, thisUrlUsed), cancellationToken);

          var query = new GetReadmeForWebQuery();
          var response = await mediator.Send(query, cancellationToken);

          
          // Mark token used and get next token
          var usedBy = context.Connection.RemoteIpAddress?.ToString() ?? "RemoteIp was empty";
          nextToken = await mediator.Send(
            new MarkTokenUsedCommand(token, thisUrlUsed, usedBy),
            cancellationToken);
          
          // Return with next token
          var result = WebOpResult.CreateSuccess("GET /api/readme", "Readme retrieved successfully", nextToken.Token, response);
          return Results.Ok(result.ToString());
        } catch (UnauthorizedAccessException ex) {
          var result = WebOpResult.CreateFailure("GET /api/readme", token, "Invalid access token.", ex);
          return Results.Problem(result.ToString());
        } catch (Exception ex) {
          var result = WebOpResult.CreateFailure("GET /api/readme", token, "Error while building readme failed.", ex);
          return Results.Problem(result.ToString());
        }
      });
      
      return app;
    }
  }
}
