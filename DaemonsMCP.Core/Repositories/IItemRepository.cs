using DaemonsMCP.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Repositories {
  public interface IItemRepository {
    public Task<List<ItemType>> GetItemTypes(string projectName); 
    public Task<ItemType> AddUpdateItemType(string projectName, ItemType itemType);
    public Task<List<StatusType>> GetStatusTypes(string projectName);

    public Task<StatusType> AddUpdateStatusType(string projectName, StatusType statusType); 

    public Task SaveProjectRepo(string projectName); 

    public Task<List<Nodes>?> GetNodes(string projectName, int? nodeId = null, int maxDepth = 1, string? statusFilter = null, string? typeFilter = null, string? nameContains = null, string? detailsContains = null);

    public Task<Nodes?> GetNodeById(string projectName, int nodeId, int maxDepth = 1, string? statusFilter = null, string? typeFilter = null, string? nameContains = null, string? detailsContains = null);

    public Task<bool> AddUpdateNodeList(string projectName, List<Nodes> listNodes);

    public Task<Nodes?> AddUpdateNode(string projectName, Nodes node);

    public Task<bool> RemoveNode(string projectName, int nodeId, RemoveStrategy removeStrategy = RemoveStrategy.PreventIfHasChildren);

  }
}
