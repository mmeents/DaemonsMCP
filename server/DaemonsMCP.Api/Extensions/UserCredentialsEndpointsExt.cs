using DaemonsMCP.Application.UserCredentials.Commands.CreateUserCredential;
using DaemonsMCP.Application.UserCredentials.Commands.UpdateUserCredential;
using DaemonsMCP.Application.UserCredentials.Commands.DeleteUserCredential;
using DaemonsMCP.Application.UserCredentials.Queries.GetUserCredentialsByUserId;
using MediatR;

namespace DaemonsMCP.Api.Extensions {
  public static class UserCredentialsEndpointsExt {
    public static WebApplication MapUserCredentialsEndpoints(this WebApplication app) {
            
      app.MapGet("/api/user-credentials", async (int userId, IMediator mediator) =>
      {
        var query = new GetUserCredentialsByUserIdQuery { UserId = userId };
        var result = await mediator.Send(query);
        return Results.Ok(result);
      });

      app.MapPost("/api/user-credentials", async (CreateUserCredentialCommand command, IMediator mediator) =>
      {
        var result = await mediator.Send(command);
        return Results.Created($"/api/user-credentials/{result.Id}", result);
      });

      app.MapPut("/api/user-credentials/{id}", async (int id, UpdateUserCredentialCommand command, IMediator mediator) =>
      {
        command.Id = id;
        var result = await mediator.Send(command);
        return Results.Ok(result);
      });

      app.MapDelete("/api/user-credentials/{id}", async (int id, IMediator mediator) =>
      {
        await mediator.Send(new DeleteUserCredentialCommand(id));
        return Results.NoContent();
      });


      return app;
    }
  }
}
