using DaemonsMCP.Application.FileSystem.Services;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Infrastructure.Persistence;
using DaemonsMCP.Infrastructure.Repositories;
using DaemonsMCP.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DaemonsMCP.Infrastructure;

public static class DependencyInjection {
  public static IServiceCollection AddInfrastructure(
      this IServiceCollection services,
      IConfiguration configuration) {

    // Add DbContext
    services.AddDbContext<DaemonsMcpDbContext>(options =>
        options.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection")));

    // Register Repositories
    services.AddScoped<ISettingRepository, SettingRepository>();
    services.AddScoped<IProjectRepository, ProjectRepository>();
    services.AddScoped<IFileSystemNodeRepository, FileSystemNodeRepository>();
    services.AddScoped<IObjectHierarchyRepository, ObjectHierarchyRepository>();
    services.AddScoped<IIdentifierRepository, IdentifierRepository>();
    services.AddScoped<IIdentifierTypeRepository, IdentifierTypeRepository>();
    services.AddScoped<IIndexQueueRepository, IndexQueueRepository>();
    services.AddScoped<IIndexingService, IndexingService>();

    // Register Services
    services.AddScoped<IFileSystemSyncService, FileSystemSyncService>();

    // Register file watching
    services.AddSingleton<IProjectFileWatcherFactory, ProjectFileWatcherFactory>();
    services.AddHostedService<FileWatcherCoordinatorService>();

    return services;
  }
}
