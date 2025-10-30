using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Extensions {
  public static class DIServiceBridge {
    private static IServiceProvider? _serviceProvider;

    public static void Initialize(IServiceProvider serviceProvider) {
      _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public static T GetService<T>() where T : notnull {
      if (_serviceProvider == null)
        throw new InvalidOperationException("DIServiceBridge not initialized. Call Initialize() first.");
      
      return _serviceProvider.GetRequiredService<T>();
    }
  }
}
