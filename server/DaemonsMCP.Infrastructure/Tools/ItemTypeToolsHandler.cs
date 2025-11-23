using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using DaemonsMCP.Application.ItemTypes.Queries.GetStatusTypes;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Constants;
using System.Text.Json;

namespace DaemonsMCP.Infrastructure.Tools {
  public class ItemTypeToolsHandler : IItemTypeToolsHandler {
    private readonly ILogger<ItemTypeToolsHandler> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    public ItemTypeToolsHandler(
        ILogger<ItemTypeToolsHandler> logger,
        IServiceScopeFactory serviceScopeFactory
    ) { 
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<string> GetStatusTypes() { 
      try { 
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        var query = new GetStatusTypesQuery();
        var result = await mediator.Send(query);
        var opResult = McpOpResult.CreateSuccess(Cx.GetStatusTypesCmd, "Status types retrieved successfully", result);           
        return JsonSerializer.Serialize(opResult);
     
      } catch (Exception ex) {
            _logger.LogError(ex, "Error getting status types");
            var opResult = McpOpResult.CreateFailure(Cx.GetStatusTypesCmd, $"Failed: {ex.Message}", null);
            return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<string> GetItemTypes() {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var query = new GetItemTypesQuery();
        var result = await mediator.Send(query);
        var opResult = McpOpResult.CreateSuccess(Cx.GetItemTypesCmd, "Item types retrieved successfully", result);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error getting item types");
        var opResult = McpOpResult.CreateFailure(Cx.GetItemByIdCmd, $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }
  }


  public interface IItemTypeToolsHandler {
    public Task<string> GetStatusTypes();
    public Task<string> GetItemTypes();

  }

}
