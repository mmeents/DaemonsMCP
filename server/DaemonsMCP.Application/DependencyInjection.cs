using DaemonsMCP.Application.FileSystem.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DaemonsMCP.Application;

public static class DependencyInjection {
  public static IServiceCollection AddApplication(this IServiceCollection services) {
    // Register MediatR and auto-discover all handlers
    services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

    return services;
  }
}