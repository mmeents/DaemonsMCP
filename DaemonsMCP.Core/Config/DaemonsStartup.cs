using DaemonsMCP.Core.Extensions;
using DaemonsMCP.Core.Repositories;
using DaemonsMCP.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;

namespace DaemonsMCP.Core.Config {

  public static class DaemonsStartup {

    public static void ConfigureDaemonsCore(this IServiceCollection services) {
      // Repositories - Core infrastructure
      services.AddSingleton<IProjectRepository, ProjectRepository>();
      services.AddSingleton<ISettingsRepository, SettingsRepository>();
      
      // Configuration - Core functionality
      services.AddSingleton<IAppConfig, App2Config>();

      // Services - Core business logic
      services.AddSingleton<ISecurityService, SecurityService>();
      services.AddSingleton<IValidationService, ValidationService>();
      services.AddSingleton<INodesRepository, NodesRepository>();
      services.AddSingleton<IItemRepository, ItemRepository>();

      services.AddSingleton<IIndexRepository, IndexRepository>();
      services.AddSingleton<IIndexService, IndexService>();

      // Scoped services - Core operations
      services.AddScoped<IProjectsService, ProjectService>();
      services.AddScoped<IProjectFolderService, ProjectFolderService>();
      services.AddScoped<IProjectFileService, ProjectFileService>();
      services.AddScoped<IClassService, ClassService>();
    }
  }

}
