using DaemonsMCP.Core.Models;
using DaemonsMCP.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Services {
  public interface IProjectsService {
    Task<IEnumerable<ProjectModel>> GetProjectsAsync();

    public Task<OperationResult> GetItemTypes() ;
    public Task<OperationResult> AddUpdateItemType(ItemType itemType);
    public Task<OperationResult> GetStatusTypes();
    public Task<OperationResult> AddUpdateStatusType(StatusType statusType);
    public Task<OperationResult> GetNodes(int? nodeId = null, int maxDepth = 1, string? statusFilter = null, string? typeFilter = null, string? nameContains = null, string? detailsContains = null);
    public Task<OperationResult> GetNodeById(int nodeId);
    public Task<OperationResult> AddUpdateNode(Nodes node);
    public Task<OperationResult> AddUpdateNodeList(List<Nodes> nodes);
    public Task<OperationResult> RemoveNode(int nodeId, RemoveStrategy removeStrategy = RemoveStrategy.PreventIfHasChildren);
    public Task<OperationResult> SaveProjectRepo();

    public Task<OperationResult> MakeTodoList(string listName, string[] items);

    public Task<OperationResult> GetNextTodoItem(string listName);

    public Task<OperationResult> MarkTodoDone(int itemId);

    public Task<OperationResult> RestoreAsTodo(int itemId);

    public Task<OperationResult> MarkTodoCancel(int itemId);

  }
}
