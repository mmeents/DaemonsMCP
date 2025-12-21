using DaemonsMCP.Application.Projects.Queries.GetAllProjects;
using DaemonsMCP.Application.Readme.Queries.GetReadme;
using DaemonsMCP.Domain.Constants;
using DaemonsMCP.Domain.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DaemonsMCP.Infrastructure.Tools;

public class ItemToolsHandler : IItemToolsHandler {
  private readonly ILogger<ItemToolsHandler> _logger;
  private readonly IServiceScopeFactory _scopeFactory;

  public ItemToolsHandler(
    ILogger<ItemToolsHandler> logger,
    IServiceScopeFactory scopeFactory) {
    _logger = logger;
    _scopeFactory = scopeFactory;
  }

  public async Task<string> GetReadme() {
    try {
      using var scope = _scopeFactory.CreateScope();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

      // Get readme items
      var readmeQuery = new GetReadmeQuery();
      var readmeResponse = await mediator.Send(readmeQuery);            

      var opResult = McpOpResult.CreateSuccess("readme", "README retrieved successfully", readmeResponse);
      return JsonSerializer.Serialize(opResult);
    } catch (Exception ex) {
      _logger.LogError(ex, "Error getting README");
      var opResult = McpOpResult.CreateFailure("readme", $"Readme retrieval failed.", null);
      return JsonSerializer.Serialize(opResult);
    }
  }


  public async Task<string> SearchItems(
    int? parentId = null,
    string? nameContains = null,
    string? detailsContains = null,
    int? typeId = null,
    int? statusId = null,
    int maxDepth = 1) {
    try {
      using var scope = _scopeFactory.CreateScope();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

      var query = new SearchItemsQuery(parentId, nameContains, detailsContains, typeId, statusId, maxDepth);
      var result = await mediator.Send(query);
      
      var opResult = McpOpResult.CreateSuccess("search-items", "Items retrieved successfully", result);
      return JsonSerializer.Serialize(opResult);
    } catch (Exception ex) {
      _logger.LogError(ex, "Error searching items");
      var opResult = McpOpResult.CreateFailure("search-items", $"Failed: {ex.Message}", null);
      return JsonSerializer.Serialize(opResult);
    }
  }

  public async Task<string> GetItemById(int itemId, int maxDepth = 1) {
    try {
      using var scope = _scopeFactory.CreateScope();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

      var query = new GetItemByIdQuery(itemId, maxDepth);
      var result = await mediator.Send(query);

      var opResult = McpOpResult.CreateSuccess("get-item-by-id", "Item retrieved successfully", result);
      return JsonSerializer.Serialize(opResult);
    } catch (Exception ex) {
      _logger.LogError(ex, "Error getting item by id {ItemId}", itemId);
      var opResult = McpOpResult.CreateFailure("get-item-by-id", $"Failed: {ex.Message}", null);
      return JsonSerializer.Serialize(opResult);
    }
  }

  public async Task<string> AddUpdateItem(
    int id,
    int? parentId,
    int itemTypeId,
    int statusTypeId,
    int rank,
    string name,
    string details,
    int? referenceFileSystemId = null,
    int? referenceObjectHierarchyId = null) {
    try {
      using var scope = _scopeFactory.CreateScope();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

      var command = new AddUpdateItemCommand(
        id, parentId, itemTypeId, statusTypeId, rank, name, details, 
        referenceFileSystemId, referenceObjectHierarchyId);
      var result = await mediator.Send(command);
      
      var opResult = McpOpResult.CreateSuccess("add-update-item", 
        id == 0 ? "Item created successfully" : "Item updated successfully", result);
      return JsonSerializer.Serialize(opResult);
    } catch (Exception ex) {
      _logger.LogError(ex, "Error adding/updating item");
      var opResult = McpOpResult.CreateFailure("add-update-item", $"Failed: {ex.Message}", null);
      return JsonSerializer.Serialize(opResult);
    }
  }

  public async Task<string> GetAllItemTypes() {
    try {
      using var scope = _scopeFactory.CreateScope();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

      var query = new GetAllItemTypesQuery();
      var result = await mediator.Send(query);
      
      var opResult = McpOpResult.CreateSuccess("get-all-item-types", "Item types retrieved successfully", result);
      return JsonSerializer.Serialize(opResult);
    } catch (Exception ex) {
      _logger.LogError(ex, "Error getting item types");
      var opResult = McpOpResult.CreateFailure("get-all-item-types", $"Failed: {ex.Message}", null);
      return JsonSerializer.Serialize(opResult);
    }
  }

  public async Task<string> AddUpdateItemType(
    int id,
    string name,
    string description,
    int rank,
    int? parentId = null) {
    try {
      using var scope = _scopeFactory.CreateScope();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

      var command = new AddUpdateItemTypeCommand(id, name, description, rank, parentId);
      var result = await mediator.Send(command);
      
      var opResult = McpOpResult.CreateSuccess("add-update-item-type", 
        id == 0 ? "Item type created successfully" : "Item type updated successfully", result);
      return JsonSerializer.Serialize(opResult);
    } catch (Exception ex) {
      _logger.LogError(ex, "Error adding/updating item type");
      var opResult = McpOpResult.CreateFailure("add-update-item-type", $"Failed: {ex.Message}", null);
      return JsonSerializer.Serialize(opResult);
    }
  }
}

public interface IItemToolsHandler {

  Task<string> GetReadme();
  Task<string> GetItemById(int itemId, int maxDepth = 1);

  Task<string> SearchItems(
    int? parentId = null,
    string? nameContains = null,
    string? detailsContains = null,
    int? typeId = null,
    int? statusId = null,
    int maxDepth = 1);
  Task<string> AddUpdateItem(
    int id,
    int? parentId,
    int itemTypeId,
    int statusTypeId,
    int rank,
    string name,
    string details,
    int? referenceFileSystemId = null,
    int? referenceObjectHierarchyId = null);

  Task<string> GetAllItemTypes();

  Task<string> AddUpdateItemType(
    int id,
    string name,
    string description,
    int rank,
    int? parentId = null);
}
