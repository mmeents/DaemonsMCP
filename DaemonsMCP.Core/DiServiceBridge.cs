using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core {
  /// <summary>
  /// Bridge between MCPSharp (which creates instances) and .NET DI container
  /// </summary>
  public static class DIServiceBridge {
    private static IServiceProvider? _serviceProvider;

    public static void Initialize(IServiceProvider serviceProvider) {
      _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public static T GetService<T>() where T : notnull {
      if (_serviceProvider == null)
        throw new InvalidOperationException("DIServiceBridge not initialized. Call Initialize() first.");

      using var scope = _serviceProvider.CreateScope();
      return scope.ServiceProvider.GetRequiredService<T>();
    }
  }

}
