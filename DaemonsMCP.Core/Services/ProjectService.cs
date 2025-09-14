using DaemonsMCP.Core.Config;
using DaemonsMCP.Core.Extensions;
using DaemonsMCP.Core.Models;
using DaemonsMCP.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DaemonsMCP.Core.Services {
  public class ProjectService : IProjectsService {
    private readonly IAppConfig _config;
    private readonly IItemRepository _itemRepository;

    public ProjectService( IAppConfig config, IItemRepository itemRepository )
    {
      _config = config;
      _itemRepository = itemRepository;
    }
    public Task<IEnumerable<ProjectModel>> GetProjectsAsync() {
      return Task.FromResult(_config.Projects.Values.AsEnumerable());
    }

    public async Task<OperationResult> GetItemTypes() { 
      var result = await _itemRepository.GetItemTypes().ConfigureAwait(false);
      if (result != null) {
          return OperationResult.CreateSuccess("GetItemTypes", "Item types retrieved successfully.", result);
      } else {
          return OperationResult.CreateFailure("GetItemTypes", "failed.");
      }
    }

    public async Task<OperationResult> AddUpdateItemType( ItemType itemType) {
      ItemType result = await _itemRepository.AddUpdateItemType( itemType).ConfigureAwait(false);
      if (result != null) {
        return OperationResult.CreateSuccess("AddUpdateItemType", "Item type added/updated successfully.", result);
      } else {
        return OperationResult.CreateFailure("AddUpdateItemType", "No Types found.");
      }      
    }

    public async Task<OperationResult> GetStatusTypes() {
      List<StatusType> result = await _itemRepository.GetStatusTypes().ConfigureAwait(false);
      if (result != null) {
        return OperationResult.CreateSuccess("GetStatusTypes", "Status types retrieved successfully.", result);
      } else {
        return OperationResult.CreateFailure("GetStatusTypes", "No Status found.");
      }
    }

    public async Task<OperationResult> AddUpdateStatusType( StatusType statusType) {
      StatusType result = await _itemRepository.AddUpdateStatusType( statusType).ConfigureAwait(false);
      if (result != null) {
        return OperationResult.CreateSuccess("AddUpdateStatusType", "Status type added/updated successfully.", result);
      } else {
        return OperationResult.CreateFailure("AddUpdateStatusType", "Failed to add update.");
      }
    }

    public async Task<OperationResult> GetNodes( int? nodeId = null, int maxDepth = 1, string? statusFilter = null, string? typeFilter = null, string? nameContains = null, string? detailsContains = null){
      List<Nodes>? result = await _itemRepository.GetNodes( nodeId, maxDepth, statusFilter, typeFilter, nameContains, detailsContains).ConfigureAwait(false);
      if (result != null) {
        return OperationResult.CreateSuccess("GetNodes", "Nodes retrieved successfully.", result);
      } else {
        return OperationResult.CreateFailure("GetNodes", $"Failed.");
      } 
    }

    public async Task<OperationResult> GetNodeById( int nodeId) {
      Nodes? result = await _itemRepository.GetNodeById( nodeId).ConfigureAwait(false);
      if (result != null) {
          return OperationResult.CreateSuccess("GetNodeById", "Node retrieved successfully.", result);
      } else {
          return OperationResult.CreateFailure("GetNodeById", $"GetNodeById failed NodeID '{nodeId}'");
      }
    }

    public async Task<OperationResult> AddUpdateNode( Nodes node) { 
      Nodes? result = await _itemRepository.AddUpdateNode( node).ConfigureAwait(false);
      if (result != null) {
        return OperationResult.CreateSuccess("AddUpdateNode", "Node added/updated successfully.", result);
      } else {
        return OperationResult.CreateFailure("AddUpdateNode", "Add Update Failed");
      }      
    }

    public async Task<OperationResult> AddUpdateNodeList( List<Nodes> nodes) {
      bool result = await _itemRepository.AddUpdateNodeList( nodes).ConfigureAwait(false);
      if (result) {
        return OperationResult.CreateSuccess("AddUpdateNode", "Node added/updated successfully.", result);
      } else {
        return OperationResult.CreateFailure("AddUpdateNode", $"Failed");
      }
    }

    public async Task<OperationResult> RemoveNode( int nodeId, RemoveStrategy removeStrategy = RemoveStrategy.PreventIfHasChildren) {
      bool result = await _itemRepository.RemoveNode( nodeId, removeStrategy).ConfigureAwait(false);
      if (result) {
        return OperationResult.CreateSuccess("RemoveNode", "Remove Node returned successfully.", result);
      } else {
        return OperationResult.CreateFailure("RemoveNode", $"Remove Node failed");
      }
    }

    public async Task<OperationResult> MakeTodoList(string listName, string[] items){
      Nodes result = await _itemRepository.MakeTodoList( listName, items).ConfigureAwait(false);
      if (result != null) {
          return OperationResult.CreateSuccess("MakeTodoList", "Make Todo List returned successfully.", result);
      } else {
          return OperationResult.CreateFailure("MakeTodoList", $"Make Todo List failed");
      }
    }

    public async Task<OperationResult> GetNextTodoItem(string? listName = null) { 
      Nodes? result = await _itemRepository.GetNextTodoItem( listName).ConfigureAwait(false);
      if (result != null) {
          return OperationResult.CreateSuccess("GetNextTodoItem", "Get Next Todo Item returned successfully.", result);
      } else {
          return OperationResult.CreateFailure("GetNextTodoItem", $"Get Next Todo Item failed");
      }
    }

    public async Task<OperationResult> MarkTodoDone(int itemId) { 
      Nodes result = await _itemRepository.MarkTodoDone( itemId).ConfigureAwait(false);
      if (result != null) {
          return OperationResult.CreateSuccess("MarkTodoDone", "Mark Todo Done returned successfully.", result);
      } else {
          return OperationResult.CreateFailure("MarkTodoDone", $"Mark Todo Done failed");
      }
    }

    public async Task<OperationResult> RestoreAsTodo(int itemId) { 
      Nodes result = await _itemRepository.RestoreAsTodo( itemId).ConfigureAwait(false);
      if (result != null) {
          return OperationResult.CreateSuccess("RestoreAsTodo", "Restore As Todo returned successfully.", result);
      } else {
          return OperationResult.CreateFailure("RestoreAsTodo", $"Restore As Todo failed");
      }
    }

    public async Task<OperationResult> SaveProjectRepo() { 
      await _itemRepository.SaveProjectRepo().ConfigureAwait(false);
      return OperationResult.CreateSuccess(Cx.SaveProjectRepoCmd, "Save Project Repo Completed Success");
    }

  }
}
