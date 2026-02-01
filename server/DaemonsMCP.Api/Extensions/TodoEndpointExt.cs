using MediatR;

using DaemonsMCP.Application.Todos.Commands.MakeTodoList;
using DaemonsMCP.Application.Todos.Commands.GetNextTodo;
using DaemonsMCP.Application.Todos.Commands.MarkTodo;

namespace DaemonsMCP.Api.Extensions {
  public static class TodoEndpointExt {
    public static WebApplication MapTodoEndpoints(this WebApplication app) {

      app.MapPost("/api/todos", (MakeTodoListCommand command,
        IMediator mediator) => { 
        return mediator.Send(command);
      })
        .WithName("MakeTodoLists")
        .WithDescription("Creates a new todo list.");

      app.MapGet("/api/todos/next-id", (
        int? todoItemId,
        IMediator mediator
        ) => {
        var command = new GetNextTodoCommand(todoItemId);
        return mediator.Send(command);
      });

      app.MapPost("/api/todos/mark-done", (MarkTodoDoneCommand command,
        IMediator mediator) => {
        return mediator.Send(command);
      });

      app.MapPost("/api/todos/mark-cancel", (MarkTodoCancelCommand command,
        IMediator mediator) => {
        return mediator.Send(command);
      });

      app.MapPost("/api/todos/restore", (RestoreAsTodoCommand command,
        IMediator mediator) => {
        return mediator.Send(command);
      });

      return app;
    }
  }
}
