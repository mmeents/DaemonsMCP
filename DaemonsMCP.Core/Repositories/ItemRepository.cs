using DaemonsMCP.Core.Config;
using DaemonsMCP.Core.Extensions;
using DaemonsMCP.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.Extensions.Logging;
using PackedTables.Net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DaemonsMCP.Core.Repositories {
  public class ItemRepository : IItemRepository {
    private IAppConfig _config;
    private readonly ConcurrentDictionary<string, NodesRepo> _projectItemRepos = new ConcurrentDictionary<string, NodesRepo>();
    private readonly NodesRepo _nodesRepo = null!;
    public ItemRepository(IAppConfig appConfig, ILoggerFactory loggerFactory) { 
        _config = appConfig;
        _nodesRepo = new NodesRepo(_config, loggerFactory);      
    }
    

    #region Nodes interface methods 

    public async Task<List<ItemType>> GetItemTypes() {
      return await Task.Run(() => {        
        var returns = _nodesRepo.GetItemTypes();
        return returns;
      }).ConfigureAwait(false);
    }

    public async Task<ItemType> AddUpdateItemType( ItemType itemType) {
      return await Task.Run(() => {        
        return _nodesRepo.AddUpdateItemType(itemType);
      }).ConfigureAwait(false);
    }


    public async Task<List<StatusType>> GetStatusTypes() {
      return await Task.Run(() => {        
        return _nodesRepo.GetItemStatusTypes();
      }).ConfigureAwait(false);
    }

    public async Task<StatusType> AddUpdateStatusType(StatusType statusType) {
      return await Task.Run(() => {        
        return _nodesRepo.AddUpdateStatusType(statusType);
      }).ConfigureAwait(false);
    }

    public async Task SaveProjectRepo() {
      await Task.Run(() => {
        _nodesRepo.WriteStorage();
       }).ConfigureAwait(false);
    }

    public async Task<List<Nodes>?> GetNodes(int? nodeId=null, int maxDepth = 1, string? statusFilter = null, string? typeFilter = null, string? nameContains = null, string? detailsContains = null) {
      return await Task.Run(() => { 
        return _nodesRepo.GetNodes(nodeId, maxDepth, statusFilter, typeFilter, nameContains, detailsContains);
      }).ConfigureAwait(false);
    }

    public async Task<Nodes?> GetNodeById(int nodeId, int maxDepth = 1, string? statusFilter = null, string? typeFilter = null, string? nameContains = null, string? detailsContains = null) {
      return await Task.Run(() => {        
        return _nodesRepo.GetNodesById(nodeId, maxDepth, statusFilter, typeFilter, nameContains, detailsContains);
      }).ConfigureAwait(false);
    }

    public async Task<bool> AddUpdateNodeList(List<Nodes> listNodes) { 
      return await Task.Run(() => {
        bool result = true;        
        foreach(var node in listNodes) {
          var updatedNode = _nodesRepo.AddUpdateNode(node);
        }
        _nodesRepo.CleanupUnusedTypes();
        _nodesRepo.WriteStorage();
        return result;
      });
    }

    public async Task<Nodes?> AddUpdateNode(Nodes node) {
      return await Task.Run(() => {        
        var updatedNode = _nodesRepo.AddUpdateNode(node);
        _nodesRepo.CleanupUnusedTypes();
        _nodesRepo.WriteStorage();
        return updatedNode;
      }).ConfigureAwait(false);
    }

    public async Task<bool> RemoveNode(int nodeId, RemoveStrategy removeStrategy = RemoveStrategy.PreventIfHasChildren) {
      return await Task.Run(() => {        
        bool result = _nodesRepo.RemoveNode(nodeId, removeStrategy);
        _nodesRepo.CleanupUnusedTypes();
        _nodesRepo.WriteStorage();
        return result;
      }).ConfigureAwait(false);
    }

    #endregion
  }


  
}
