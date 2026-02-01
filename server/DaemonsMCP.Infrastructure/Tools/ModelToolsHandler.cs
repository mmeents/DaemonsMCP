using DaemonsMCP.Application.Models.Commands.AddUpdateModel;
using DaemonsMCP.Application.Models.Commands.DeleteModel;
using DaemonsMCP.Application.Models.Queries.GetModelById;
using DaemonsMCP.Application.Models.Queries.SearchModels;
using DaemonsMCP.Application.Templates.Commands.ExecuteTemplate;
using DaemonsMCP.Domain.Constants;
using DaemonsMCP.Domain.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;



namespace DaemonsMCP.Infrastructure.Tools {
  public class ModelToolsHandler : IModelToolsHandler {
    private readonly ILogger<ModelToolsHandler> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ModelToolsHandler(
        ILogger<ModelToolsHandler> logger,
        IServiceScopeFactory serviceScopeFactory
    ) {
      _logger = logger;
      _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<string> SearchModelsAsync(
        int projectId,
        int? parentId = null,
        int? modelTypeId = null,
        string? nameFilter = null,
        int maxDepth = 1,
        bool includeProperties = false
    ) {
      try { 
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
    
        var query = new SearchModelsQuery(
            projectId,
            parentId,
            modelTypeId,
            nameFilter,
            maxDepth,
            includeProperties
        );
    
        var result = await mediator.Send(query);
        var opResult = McpOpResult.CreateSuccess(Cx.SearchModelsCmd, "Items retrieved successfully", result);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error occurred while searching models");
        var opResult = McpOpResult.CreateFailure(Cx.SearchModelsCmd, $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<string> GetModelByIdAsync(
        int modelId,
        int maxDepth = 1,
        bool includeProperties = false
    ) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var query = new GetModelByIdQuery( modelId, maxDepth,includeProperties );
        var result = await mediator.Send(query);
        var opResult = McpOpResult.CreateSuccess(Cx.GetModelCmd, "Model retrieved successfully", result);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error occurred while retrieving model by ID");
        var opResult = McpOpResult.CreateFailure(Cx.GetModelCmd, $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<string> AddUpdateModelAsync(
        int id,
        int projectId,
        int? parentId,
        int modelTypeId,
        string name,
        int rank,
        string? code = null
    ) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var command = new AddUpdateModelCommand( id, projectId, parentId, modelTypeId, name, rank, code );
        var result = await mediator.Send(command);
        var opResult = McpOpResult.CreateSuccess(Cx.AddUpdateModelCmd, "Model added/updated successfully", result);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error occurred while adding/updating model");
        var opResult = McpOpResult.CreateFailure(Cx.AddUpdateModelCmd, $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<string> AddUpdateModelPropertyAsync(
        int id,
        int modelId,
        string propertyKey,
        string? propertyValue = null,
        int? propertyValueTypeId = null
      ) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var command = new AddUpdateModelPropertyCommand(id, modelId, propertyKey, propertyValue, propertyValueTypeId );
        var result = await mediator.Send(command);
        var opResult = McpOpResult.CreateSuccess(Cx.AddUpdateModelPropertyCmd, "Model property added/updated successfully", result);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error occurred while adding/updating model property");
        var opResult = McpOpResult.CreateFailure(Cx.AddUpdateModelPropertyCmd, $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<string> DeleteModelAsync(int modelId) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var command = new DeleteModelCommand(modelId);
        var result = await mediator.Send(command);
        var opResult = McpOpResult.CreateSuccess(Cx.DeleteModelCmd, "Model deleted successfully", result);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error occurred while deleting model");
        var opResult = McpOpResult.CreateFailure(Cx.DeleteModelCmd, $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<string> DeleteModelPropertyAsync(int propertyId) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var command = new DeleteModelPropertyCommand(propertyId);
        var result = await mediator.Send(command);
        var opResult = McpOpResult.CreateSuccess(Cx.DeleteModelPropertyCmd, "Model property deleted successfully", result);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error occurred while deleting model property");
        var opResult = McpOpResult.CreateFailure(Cx.DeleteModelPropertyCmd, $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<string> ExecuteTemplateAsync(ExecuteTemplateCommand command) { 
      try {         
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(command);
        var opResult = McpOpResult.CreateSuccess(Cx.ExecuteTemplateCmd, "Template executed successfully", result);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error occurred while executing template");
        var opResult = McpOpResult.CreateFailure(Cx.ExecuteTemplateCmd, $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }


  }

  interface IModelToolsHandler {
    Task<string> SearchModelsAsync(
        int projectId,
        int? parentId = null,
        int? modelTypeId = null,
        string? nameFilter = null,
        int maxDepth = 1,
        bool includeProperties = false
    );
    Task<string> GetModelByIdAsync(
        int modelId,
        int maxDepth = 1,
        bool includeProperties = false
    );
    Task<string> AddUpdateModelAsync(
        int id,
        int projectId,
        int? parentId,
        int modelTypeId,
        string name,
        int rank,
        string? code = null
    );
    Task<string> AddUpdateModelPropertyAsync(
        int id,
        int modelId,
        string propertyKey,
        string? propertyValue = null,
        int? propertyValueTypeId = null
      );

    Task<string> DeleteModelAsync(int modelId);
    Task<string> DeleteModelPropertyAsync(int propertyId);

    Task<string> ExecuteTemplateAsync(ExecuteTemplateCommand command);

  }
}
