using DaemonsMCP.Application.FileSystem.Services;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Infrastructure.Persistence;
using DaemonsMCP.Infrastructure.Repositories;
using DaemonsMCP.Infrastructure.Services;
using DaemonsMCP.Infrastructure.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DaemonsMCP.Infrastructure;

public static class DependencyInjection {

  public static IServiceCollection AddWebInfra (this IServiceCollection services, IConfiguration configuration) {
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
    services.AddScoped<IItemRepository, ItemRepository>();
    services.AddScoped<IItemTypeRepository, ItemTypeRepository>();
    services.AddScoped<IAccessTokenRepository, AccessTokenRepository>();

    // Register Services
    services.AddScoped<IIndexingService, IndexingService>();
    services.AddScoped<IValidationService, ValidationService>();
    services.AddScoped<IFileSystemSyncService, FileSystemSyncService>();
    services.AddScoped<IDatabaseManagementService, DatabaseManagementService>();

    // Register file watching
    services.AddSingleton<IProjectFileWatcherFactory, ProjectFileWatcherFactory>();
    services.AddHostedService<FileWatcherHostedService>(); // Add any web-specific infrastructure services here if needed in the future
    return services;
  }

  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {

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
    services.AddScoped<IItemRepository, ItemRepository>();
    services.AddScoped<IItemTypeRepository, ItemTypeRepository>();
    services.AddScoped<IAccessTokenRepository, AccessTokenRepository>();

    // Register Services
    services.AddScoped<IIndexingService, IndexingService>();
    services.AddScoped<IValidationService, ValidationService>();
    services.AddScoped<IFileSystemSyncService, FileSystemSyncService>();
    services.AddScoped<IDatabaseManagementService,  DatabaseManagementService >();

    // Register file watching
    services.AddSingleton<IProjectFileWatcherFactory, ProjectFileWatcherFactory>();
    services.AddHostedService<FileWatcherHostedService>();

    // Register Mcp Tools
    services.AddSingleton<IProjectToolsHandler, ProjectToolsHandler>();
    services.AddSingleton<IFileSystemToolsHandler, FileSystemToolsHandler>();
    services.AddSingleton<IObjectHierarchyToolsHandler, ObjectHierarchyToolsHandler>();
    services.AddSingleton<IItemToolsHandler, ItemToolsHandler>();
    services.AddSingleton<IItemTypeToolsHandler, ItemTypeToolsHandler>();

    services.AddHostedService<McpServerHostedService>();

    return services;
  }
}
