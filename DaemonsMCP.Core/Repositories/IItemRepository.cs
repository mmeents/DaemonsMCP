using DaemonsMCP.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Repositories {
  public interface IItemRepository {
    public Task<List<ItemType>> GetItemTypes(); 
    public Task<ItemType> AddUpdateItemType( ItemType itemType);
    public Task<List<StatusType>> GetStatusTypes();

    public Task<StatusType> AddUpdateStatusType(StatusType statusType); 

    public Task SaveProjectRepo(); 

    public Task<List<Nodes>?> GetNodes(int? nodeId = null, int maxDepth = 1, string? statusFilter = null, string? typeFilter = null, string? nameContains = null, string? detailsContains = null);

    public Task<Nodes?> GetNodeById(int nodeId, int maxDepth = 1, string? statusFilter = null, string? typeFilter = null, string? nameContains = null, string? detailsContains = null);

    public Task<bool> AddUpdateNodeList(List<Nodes> listNodes);

    public Task<Nodes?> AddUpdateNode(Nodes node);

    public Task<bool> RemoveNode(int nodeId, RemoveStrategy removeStrategy = RemoveStrategy.PreventIfHasChildren);


    public Task<Nodes> MakeTodoList(string listName, string[] items);
    
    public Task<Nodes?> GetNextTodoItem(string listName);

    public Task<Nodes> MarkTodoDone(int itemId);

    public Task<Nodes> RestoreAsTodo(int itemId);

    public Task<Nodes> MarkTodoCancel(int itemId);

  }
}
