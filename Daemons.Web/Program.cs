using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Core;
using Daemons.Web.Extensions;
using DaemonsMCP.Application;
using DaemonsMCP.Infrastructure;
using DaemonsMCP.Domain.Extensions;

namespace Daemons.Web {
  public class Program {
    public static void Main(string[] args) {
      ConfigureSerilog();
      var builder = WebApplication.CreateBuilder(args);

      // Configure to use daemonsmcp.json instead of appsettings.json
      builder.Configuration.Sources.Clear();
      builder.Configuration
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("daemonsmcp.json", optional: false, reloadOnChange: true)
          .AddEnvironmentVariables()
          .AddCommandLine(args);

      // Use host integration so Serilog registers required services like DiagnosticContext
      builder.Host.UseSerilog();

      builder.Services.AddMvc();

      builder.Services.AddApplication();
      builder.Services.AddWebInfra(builder.Configuration);

      builder.Services.AddEndpointsApiExplorer();
      builder.Services.AddSwaggerGen(options => {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "Daemons.Web", Version = "v1" });
      });

      builder.Services.AddCors(options =>
      {
        options.AddDefaultPolicy(policy =>
        {
          policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
      });

      var app = builder.Build();

      app.UseCors();
      app.UseHttpsRedirection();

      // REQUEST LOGGING - See every request with full context
      app.UseSerilogRequestLogging(options =>
      {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
          diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");
          diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
          diagnosticContext.Set("QueryString", httpContext.Request.QueryString.Value);
          diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);

          // Token tracking (don't log actual token value for security!)
          var hasToken = httpContext.Request.Query.ContainsKey("token");
          diagnosticContext.Set("HasAuthToken", hasToken);

          if (hasToken)
          {
            var token = httpContext.Request.Query["token"].ToString();
            // Log first/last 4 chars for debugging without exposing full token
            if (token.Length >= 8)
            {
              diagnosticContext.Set("TokenHint", $"{token.Substring(0, 4)}...{token.Substring(token.Length - 4)}");
            }
            else
            {
              diagnosticContext.Set("TokenHint", "short-token");
            }
          }
        };

        // Smart log levels - don't spam with static files
        options.GetLevel = (httpContext, elapsed, ex) =>
        {
          // Suppress static file requests
          if (httpContext.Request.Path.StartsWithSegments("/css") ||
              httpContext.Request.Path.StartsWithSegments("/js") ||
              httpContext.Request.Path == "/favicon.ico")
          {
            return Serilog.Events.LogEventLevel.Verbose; // Won't log unless Verbose is enabled
          }

          // Errors
          if (ex != null || httpContext.Response.StatusCode >= 500)
            return Serilog.Events.LogEventLevel.Error;

          // Client errors (including auth failures!)
          if (httpContext.Response.StatusCode >= 400)
            return Serilog.Events.LogEventLevel.Warning;

          return Serilog.Events.LogEventLevel.Information;
        };
      });

      // Serve static files early in pipeline for better performance
      app.UseDefaultFiles();
      app.UseStaticFiles();

      app.MapGet("/health", () => {
        return Results.Ok("Healthy");
      });

      app.MapProjectEndpoints()
         .MapFileSystemEndpoints();

      app.UseSwagger();
      app.UseSwaggerUI(options =>
      {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Daemons.Web V1");
        options.RoutePrefix = "swagger"; // Instead of empty string
      });

      app.Run();
    }

    private static void ConfigureSerilog() {
      // Ensure logs directory exists
      var logsPath = CommonPath.LogsAppPath;

      Log.Logger = new LoggerConfiguration()
          .MinimumLevel.Debug()
          .MinimumLevel.Override("Microsoft", new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Warning)) // Reduce Microsoft logging noise
          .MinimumLevel.Override("System", new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Warning))
          .Enrich.FromLogContext()
          .WriteTo.File(
              path: Path.Combine(logsPath, "Daemons.web.log"),
              rollingInterval: RollingInterval.Day,
              retainedFileCountLimit: 7,
              outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
          )
          .WriteTo.File(
              path: Path.Combine(logsPath, "Daemons.web-errors.log"),
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
