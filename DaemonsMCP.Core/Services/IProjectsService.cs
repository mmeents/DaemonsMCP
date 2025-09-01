using DaemonsMCP.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Services {
  public interface IProjectsService {
    Task<IEnumerable<ProjectModel>> GetProjectsAsync();

    public Task<OperationResult> GetItemTypes(string projectName) ;
    public Task<OperationResult> AddUpdateItemType(string projectName, ItemType itemType);
    public Task<OperationResult> GetStatusTypes(string projectName);
    public Task<OperationResult> AddUpdateStatusType(string projectName, StatusType statusType);
    public Task<OperationResult> GetNodes(string projectName, int? nodeId = null, int maxDepth = 1, string? statusFilter = null, string? typeFilter = null, string? nameContains = null, string? detailsContains = null);
    public Task<OperationResult> GetNodeById(string projectName, int nodeId);
    public Task<OperationResult> AddUpdateNode(string projectName, Nodes node);
    public Task<OperationResult> AddUpdateNodeList(string projectName, List<Nodes> nodes);
    public Task<OperationResult> RemoveNode(string projectName, int nodeId, RemoveStrategy removeStrategy = RemoveStrategy.PreventIfHasChildren);
    public Task<OperationResult> SaveProjectRepo(string projectName);

  }
}
