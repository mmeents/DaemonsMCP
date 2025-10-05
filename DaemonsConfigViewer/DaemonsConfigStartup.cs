using DaemonsConfigViewer.Models;
using DaemonsMCP.Core.Config;
using DaemonsMCP.Core.Repositories;
using DaemonsMCP.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsConfigViewer {
  public static class ConfigViewerStartup {
    /// <summary>
    /// ConfigViewer specific services (WinForms app)
    /// </summary>
    public static IServiceCollection AddConfigViewer(this IServiceCollection services) {
      // Add core services
      services.ConfigureDaemonsCore();

      // ConfigViewer-specific providers
      services.AddScoped<IStatusProvider, StatusProvider>();      
      services.AddScoped<ITypeProvider, TypeProvider>();
      services.AddScoped<IParentItemProvider, ParentItemProvider>();

      return services;
    }
  }
}
