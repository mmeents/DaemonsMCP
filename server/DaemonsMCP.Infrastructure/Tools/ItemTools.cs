using DaemonsMCP.Domain.Constants;
using DaemonsMCP.Domain.Extensions;
using MCPSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Infrastructure.Tools {  

  public class ItemTools {
    private static IItemToolsHandler GetTool() => DIServiceBridge.GetService<IItemToolsHandler>();

    [McpTool(Cx.GetReadMeCmd, Cx.GetReadMeCmdDesc)]
    public static async Task<object> GetReadMe() {
      return await GetTool().SearchItems(parentId: null, null, null, Cx.ItemTypeIdReadme, null,  maxDepth: 4);
    }


    [McpTool(Cx.SearchItemsCmd, Cx.SearchItemCmdDesc)]
    public static async Task<string> SearchItems(
        [Description(Cx.SearchParentIdParamDesc)] int? parentId = null,
        [Description(Cx.NameContainsParamDesc)] string? nameContains = null,
        [Description(Cx.DetailsContainsParamDesc)] string? detailsContains = null,
        [Description(Cx.SearchTypeIdParamDesc)] int? typeId = null,
        [Description(Cx.SearchStatusIdParamDesc)] int? statusId = null,
        [Description(Cx.MaxDepthParamDesc)] int maxDepth = 1) {
      return await GetTool().SearchItems(parentId, nameContains, detailsContains, typeId, statusId, maxDepth);
    }


    [McpTool(Cx.GetItemByIdCmd, Cx.GetItemByIdCmdDesc)]
    public static async Task<string> GetItemById(
       [Description(Cx.ItemIdParamDesc)] int itemId,
       [Description(Cx.MaxDepthParamDesc)] int maxDepth = 1) {
      return await GetTool().GetItemById(itemId, maxDepth);
    }


    [McpTool(Cx.AddUpdateItemCmd, Cx.AddUpdateItemCmdDesc)]
    public static async Task<string> AddUpdateItem(
        [Description(Cx.ItemIdParamDesc)] int id,
        [Description(Cx.ParentIdParamDesc)] int? parentId,
        [Description(Cx.TypeIdParamDesc)] int itemTypeId,
        [Description(Cx.StatusIdParamDesc)] int statusTypeId,
        [Description(Cx.RankParamDesc)] int rank,
        [Description(Cx.NameParamDesc)] string name,
        [Description(Cx.DetailsParamDesc)] string details,
        [Description(Cx.RefFileSystemIdParamDesc)] int? referenceFileSystemId = null,
        [Description(Cx.RefObjectHierarchyIdParamDesc)] int? referenceObjectHierarchyId = null) {
      return await GetTool().AddUpdateItem(
          id, parentId, itemTypeId, statusTypeId, rank, name, details,
          referenceFileSystemId, referenceObjectHierarchyId);
    }

    
  }
}
