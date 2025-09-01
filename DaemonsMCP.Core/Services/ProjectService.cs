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

    public async Task<OperationResult> GetItemTypes(string projectName) { 
      var result = await _itemRepository.GetItemTypes(projectName).ConfigureAwait(false);
      if (result != null) {
          return OperationResult.CreateSuccess("GetItemTypes", "Item types retrieved successfully.", result);
      } else {
          return OperationResult.CreateFailure("GetItemTypes", $"Project '{projectName}' not found.");
      }
    }

    public async Task<OperationResult> AddUpdateItemType(string projectName, ItemType itemType) {
      ItemType result = await _itemRepository.AddUpdateItemType(projectName, itemType).ConfigureAwait(false);
      if (result != null) {
        return OperationResult.CreateSuccess("AddUpdateItemType", "Item type added/updated successfully.", result);
      } else {
        return OperationResult.CreateFailure("AddUpdateItemType", $"Project '{projectName}' No Types found.");
      }      
    }

    public async Task<OperationResult> GetStatusTypes(string projectName) {
      List<StatusType> result = await _itemRepository.GetStatusTypes(projectName).ConfigureAwait(false);
      if (result != null) {
        return OperationResult.CreateSuccess("GetStatusTypes", "Status types retrieved successfully.", result);
      } else {
        return OperationResult.CreateFailure("GetStatusTypes", $"Project '{projectName}' No Status found.");
      }
    }

    public async Task<OperationResult> AddUpdateStatusType(string projectName, StatusType statusType) {
      StatusType result = await _itemRepository.AddUpdateStatusType(projectName, statusType).ConfigureAwait(false);
      if (result != null) {
        return OperationResult.CreateSuccess("AddUpdateStatusType", "Status type added/updated successfully.", result);
      } else {
        return OperationResult.CreateFailure("AddUpdateStatusType", $"Project '{projectName}' failed to add update.");
      }
    }

    public async Task<OperationResult> GetNodes(string projectName, int? nodeId = null, int maxDepth = 1, string? statusFilter = null, string? typeFilter = null, string? nameContains = null, string? detailsContains = null){
      List<Nodes>? result = await _itemRepository.GetNodes(projectName, nodeId, maxDepth, statusFilter, typeFilter, nameContains, detailsContains).ConfigureAwait(false);
      if (result != null) {
        return OperationResult.CreateSuccess("GetNodes", "Nodes retrieved successfully.", result);
      } else {
        return OperationResult.CreateFailure("GetNodes", $"Project '{projectName}' not found.");
      } 
    }

    public async Task<OperationResult> GetNodeById(string projectName, int nodeId) {
      Nodes? result = await _itemRepository.GetNodeById(projectName, nodeId).ConfigureAwait(false);
      if (result != null) {
          return OperationResult.CreateSuccess("GetNodeById", "Node retrieved successfully.", result);
      } else {
          return OperationResult.CreateFailure("GetNodeById", $"Project'{projectName}' not found or Node ID '{nodeId}' does not exist.");
      }
    }

    public async Task<OperationResult> AddUpdateNode(string projectName, Nodes node) { 
      Nodes? result = await _itemRepository.AddUpdateNode(projectName, node).ConfigureAwait(false);
      if (result != null) {
        return OperationResult.CreateSuccess("AddUpdateNode", "Node added/updated successfully.", result);
      } else {
        return OperationResult.CreateFailure("AddUpdateNode", $"Project '{projectName}' not found.");
      }      
    }

    public async Task<OperationResult> AddUpdateNodeList(string projectName, List<Nodes> nodes) {
      bool result = await _itemRepository.AddUpdateNodeList(projectName, nodes).ConfigureAwait(false);
      if (result) {
        return OperationResult.CreateSuccess("AddUpdateNode", "Node added/updated successfully.", result);
      } else {
        return OperationResult.CreateFailure("AddUpdateNode", $"Project '{projectName}' not found.");
      }
    }

    public async Task<OperationResult> RemoveNode(string projectName, int nodeId, RemoveStrategy removeStrategy = RemoveStrategy.PreventIfHasChildren) {
      bool result = await _itemRepository.RemoveNode(projectName, nodeId, removeStrategy).ConfigureAwait(false);
      if (result) {
        return OperationResult.CreateSuccess("AddUpdateNode", "Node added/updated successfully.", result);
      } else {
        return OperationResult.CreateFailure("AddUpdateNode", $"Project '{projectName}' not found.");
      }
    }


    public async Task<OperationResult> SaveProjectRepo(string projectName) { 
      await _itemRepository.SaveProjectRepo(projectName).ConfigureAwait(false);
      return OperationResult.CreateSuccess(Cx.SaveProjectRepoCmd, "Save Project Repo Completed Success");
    }

  }
}
