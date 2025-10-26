using DaemonsMCP.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Infrastructure.Services {
  public interface IProjectFileWatcherFactory {
    ProjectFileWatcherService Create(int projectId, string projectPath);
  }

  public class ProjectFileWatcherFactory : IProjectFileWatcherFactory {
    private readonly IServiceScopeFactory _scopeFactory; // Change to scopeFactory
    private readonly ILoggerFactory _loggerFactory;

    public ProjectFileWatcherFactory(
        IServiceScopeFactory scopeFactory,
        ILoggerFactory loggerFactory) {
      _scopeFactory = scopeFactory;
      _loggerFactory = loggerFactory;
    }

    public ProjectFileWatcherService Create(int projectId, string projectPath) {
      // Create a scope to resolve scoped dependencies
      var logger = _loggerFactory.CreateLogger<ProjectFileWatcherService>();

      return new ProjectFileWatcherService(
          projectId,
          projectPath,
          _scopeFactory,          
          logger);
    }
  }
}
