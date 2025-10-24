using DaemonsMCP.Api.Extensions;
using DaemonsMCP.Application;
using DaemonsMCP.Application.FileSystem.Commands.SyncProjectFileSystem;
using DaemonsMCP.Application.Projects.Commands.CreateProject;
using DaemonsMCP.Application.Projects.Queries.GetAllProjects;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Infrastructure;
using DaemonsMCP.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Core;
using Swashbuckle.AspNetCore.Swagger;

namespace DaemonsMCP.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigureSerilog();
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog();
            builder.Services.AddMvc();

            builder.Services.AddApplication();
            
            // This line is CRITICAL for migrations to work
            builder.Services.AddInfrastructure(builder.Configuration);

            builder.Services.AddSwaggerGen(options => { 
              options.SwaggerDoc("v1", new OpenApiInfo { Title = "DaemonsMCP API", Version = "v4" });
            });

            var app = builder.Build();

            // Configure middleware
            if (app.Environment.IsDevelopment()) {
              app.MapSwagger();
              app.UseSwagger();
              app.UseSwaggerUI(options =>
              {
                options.SwaggerEndpoint("v1/swagger.json", "DaemonsMCP.API V1");
              });
            }
            
       
            app.UseHttpsRedirection();


            app.MapGet("/health", () => { 
                return Results.Ok("Healthy");
            });

            app.MapProjectEndpoints()
               .MapFileSystemEndpoints()
               .MapIndexingEndpoints();

            app.Run();
        }

    private static void ConfigureSerilog() {
      // Ensure logs directory exists
      var logsPath = "c:\\MCPSandbox\\logs"; // Sx.LogsAppPath;

      Log.Logger = new LoggerConfiguration()
          .MinimumLevel.Debug()
          .MinimumLevel.Override("Microsoft", new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Warning)) // Reduce Microsoft logging noise
          .MinimumLevel.Override("System", new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Warning))
          .Enrich.FromLogContext()
          .WriteTo.File(
              path: Path.Combine(logsPath, "D3MCP-.log"),
              rollingInterval: RollingInterval.Day,
              retainedFileCountLimit: 7,
              outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
          )
          .WriteTo.File(
              path: Path.Combine(logsPath, "D3MCP-errors-.log"),
              rollingInterval: RollingInterval.Day,
              retainedFileCountLimit: 30,
              restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning, // Only warnings and errors
              outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
          )
          .CreateLogger();

      Log.Information("Serilog configured - logging to {LogsPath}", logsPath);
    }
  }
}
