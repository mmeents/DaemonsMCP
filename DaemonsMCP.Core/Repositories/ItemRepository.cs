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
    private readonly ConcurrentDictionary<string, ProjectItemRepo> _projectItemRepos = new ConcurrentDictionary<string, ProjectItemRepo>();
    public ItemRepository(IAppConfig appConfig, ILoggerFactory loggerFactory) { 
        _config = appConfig;
        foreach (var project in _config.Projects) {
          _projectItemRepos[project.Key] = new ProjectItemRepo(project.Value, _config, loggerFactory);
      }
    }
    public ProjectItemRepo? GetProjectRepo(string projectName) {
      if (string.IsNullOrEmpty(projectName)) return null;
      if (_projectItemRepos.TryGetValue(projectName, out ProjectItemRepo? repo)) {
        return repo;
      }
      return null;
    }

    #region Nodes interface methods 

    public async Task<List<ItemType>> GetItemTypes(string projectName) {
      return await Task.Run(() => {
        var projectRepo = GetProjectRepo(projectName);
        if (projectRepo == null) return new List<ItemType>();
        var returns = projectRepo.GetItemTypes();
        return returns;
      }).ConfigureAwait(false);
    }

    public async Task<ItemType> AddUpdateItemType(string projectName, ItemType itemType) {
      return await Task.Run(() => {
        var projectRepo = GetProjectRepo(projectName);
        if (projectRepo == null) throw new ArgumentException($"Project '{projectName}' not found.");
        return projectRepo.AddUpdateItemType(itemType);
      }).ConfigureAwait(false);
    }


    public async Task<List<StatusType>> GetStatusTypes(string projectName) {
      return await Task.Run(() => {
        var projectRepo = GetProjectRepo(projectName);
        if (projectRepo == null) return new List<StatusType>();
        return projectRepo.GetItemStatusTypes();
      }).ConfigureAwait(false);
    }

    public async Task<StatusType> AddUpdateStatusType(string projectName, StatusType statusType) {
      return await Task.Run(() => {
        var projectRepo = GetProjectRepo(projectName);
        if (projectRepo == null) throw new ArgumentException($"Project '{projectName}' not found.");
        return projectRepo.AddUpdateStatusType(statusType);
      }).ConfigureAwait(false);
    }

    public async Task SaveProjectRepo(string projectName) {
      await Task.Run(() => {
        var projectRepo = GetProjectRepo(projectName);
        if (projectRepo != null) {
          projectRepo.WriteStorage();
        }
      }).ConfigureAwait(false);
    }

    public async Task<List<Nodes>?> GetNodes(string projectName, int? nodeId=null, int maxDepth = 1, string? statusFilter = null, string? typeFilter = null, string? nameContains = null, string? detailsContains = null) {
      return await Task.Run(() => {
        var projectRepo = GetProjectRepo(projectName);
        if (projectRepo == null) return null;
        return projectRepo.GetNodes(nodeId, maxDepth, statusFilter, typeFilter, nameContains, detailsContains);
      }).ConfigureAwait(false);
    }

    public async Task<Nodes?> GetNodeById(string projectName, int nodeId, int maxDepth = 1, string? statusFilter = null, string? typeFilter = null, string? nameContains = null, string? detailsContains = null) {
      return await Task.Run(() => {
        var projectRepo = GetProjectRepo(projectName);
        if (projectRepo == null) return null;
        return projectRepo.GetNodesById(nodeId, maxDepth, statusFilter, typeFilter, nameContains, detailsContains);
      }).ConfigureAwait(false);
    }

    public async Task<bool> AddUpdateNodeList(string projectName, List<Nodes> listNodes) { 
      return await Task.Run(() => {
        bool result = true;
        var projectRepo = GetProjectRepo(projectName);
        if (projectRepo == null) return false;
        foreach(var node in listNodes) {
          var updatedNode = projectRepo.AddUpdateNode(node);
        }
        projectRepo.CleanupUnusedTypes();
        projectRepo.WriteStorage();
        return result;
      });
    }

    public async Task<Nodes?> AddUpdateNode(string projectName, Nodes node) {
      return await Task.Run(() => {
        var projectRepo = GetProjectRepo(projectName);
        if (projectRepo == null) return null;
        var updatedNode = projectRepo.AddUpdateNode(node);
        projectRepo.CleanupUnusedTypes();
        projectRepo.WriteStorage();
        return updatedNode;
      }).ConfigureAwait(false);
    }

    public async Task<bool> RemoveNode(string projectName, int nodeId, RemoveStrategy removeStrategy = RemoveStrategy.PreventIfHasChildren) {
      return await Task.Run(() => {
        var projectRepo = GetProjectRepo(projectName);
        if (projectRepo == null) return false;
        bool result = projectRepo.RemoveNode(nodeId, removeStrategy);
        projectRepo.CleanupUnusedTypes();
        projectRepo.WriteStorage();
        return result;
      }).ConfigureAwait(false);
    }

    #endregion
  }


  
}
