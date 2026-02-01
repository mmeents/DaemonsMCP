using MediatR;
using DaemonsMCP.Application.Utilities.Commands;

namespace DaemonsMCP.Api.Extensions {
  public static class SystemExt {
    public static WebApplication MapSystemEndpoints(this WebApplication app) {
      app.MapGet("/api/system/info", (
        IMediator mediator
        ) => {
        var command = new GetSystemInformationCommand();
        return mediator.Send(command);
      })
      .WithName("GetSystemInformation")
      .WithDescription("Retrieves system information about the server environment.");
      return app;
    }
  }
}
