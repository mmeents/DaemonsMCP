using MediatR;
using Microsoft.AspNetCore.Mvc;
using DaemonsMCP.Application.Templates.Commands.ExecuteTemplate;



namespace DaemonsMCP.Api.Extensions {
  public static class TemplatesEndpointExt {
    public static WebApplication MapTemplatesEndpoints(this WebApplication app) {

      app.MapPost("/api/templates/{templateId}/execute", async (int templateId, IMediator mediator) => {
          try {
            var command = new ExecuteTemplateCommand(templateId);
            var result = await mediator.Send(command);
            return Results.Ok(result);
          } catch (Exception ex) {
            return Results.BadRequest(new {
              Success = false,
              Error = ex.Message
            });
          }
        });
      

      return app;
    }
  }
}
