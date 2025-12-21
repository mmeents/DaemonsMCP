using DaemonsMCP.Application.Users.Commands.AuthenticateUser;
using DaemonsMCP.Application.Users.Commands.CreateUser;
using DaemonsMCP.Application.Users.Commands.GetUserById;  
using DaemonsMCP.Application.Users.Commands.GetAllUsers;
using MediatR;

namespace DaemonsMCP.Api.Extensions {
  public static class UserEndpointExt {
    public static WebApplication MapUserEndpoints(this WebApplication app) {

      // Register new user
      app.MapPost("/api/users/register", async (
        CreateUserCommand command,
        IMediator mediator) => {

          try {
            var result = await mediator.Send(command);
            return Results.Created($"/api/users/{result.Id}", new { success = true, data = result });
          } catch (InvalidOperationException ex) {
            return Results.BadRequest(new { success = false, errorMessage = ex.Message });
          }
        }).WithName("RegisterUser");

      // Login
      app.MapPost("/api/users/login", async (
        AuthenticateUserCommand command,
        IMediator mediator) => {

          var result = await mediator.Send(command);
          return result is not null
          ? Results.Ok(new { success = true, data = result })
          : Results.Unauthorized();
        }).WithName("LoginUser");

      // Get user by ID
      app.MapGet("/api/users/{id}", async (
        int id,
        IMediator mediator) => {

          var query = new GetUserByIdQuery(id);
          var user = await mediator.Send(query);

          return user is not null
          ? Results.Ok(new { success = true, data = user })
          : Results.NotFound(new { success = false, errorMessage = "User not found" });
        }).WithName("GetUserById");

      app.MapGet("/api/users", async (        
        IMediator mediator) => {
          var command = new GetAllUsersCommand();
          var result = await mediator.Send(command);
          return Results.Ok(new { success = true, data = result });
        }).WithName("GetAllUsers");

      return app;
    }
  }
}
