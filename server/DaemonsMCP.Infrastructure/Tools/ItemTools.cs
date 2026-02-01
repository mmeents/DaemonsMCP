using DaemonsMCP.Domain.Constants;
using DaemonsMCP.Domain.Extensions;
using MCPSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaemonsMCP.Application.Todos.Commands.MakeTodoList;
using DaemonsMCP.Application.Todos.Commands.GetNextTodo;
using DaemonsMCP.Application.Todos.Commands.MarkTodo;

namespace DaemonsMCP.Infrastructure.Tools {  

  public class ItemTools {
    private static IItemToolsHandler GetTool() => DIServiceBridge.GetService<IItemToolsHandler>();

    [McpTool(Cx.GetReadMeCmd, Cx.GetReadMeCmdDesc)]
    public static async Task<object> GetReadMe() {
      return await GetTool().GetReadme();
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

    #region Todo Operations

    [McpTool(Cx.MakeTodoListCmd, Cx.MakeTodoListCmdDesc)]
    public static async Task<object> MakeTodoList(
      [Description(Cx.ListNameParamDesc)] string listName,
      [Description(Cx.ItemsParamDesc)] string[] items
    ) {
      var command = new MakeTodoListCommand(listName, items);
      return await GetTool().MakeTodoList(command).ConfigureAwait(false);
    }

    [McpTool(Cx.GetNextTodoItemCmd, Cx.GetNextTodoItemCmdDesc)]
    public static async Task<object> GetNextTodoItem(
      [Description(Cx.ListItemIdParamDesc)] int? listItemId
    ) {      
      return await GetTool().GetNextTodoItem(listItemId).ConfigureAwait(false);
    }

    [McpTool(Cx.MarkTodoDoneCmd, Cx.MarkTodoDoneCmdDesc)]
    public static async Task<object> MarkTodoDone(
      [Description(Cx.ItemIdParamDesc)] int itemId
    ) => await GetTool().MarkTodoDone(new MarkTodoDoneCommand(itemId)).ConfigureAwait(false);

    [McpTool(Cx.RestoreAsTodoCmd, Cx.RestoreAsTodoCmdDesc)]
    public static async Task<object> RestoreAsTodo(
      [Description(Cx.ItemIdParamDesc)] int itemId
    ) => await GetTool().RestoreAsTodo(new RestoreAsTodoCommand(itemId)).ConfigureAwait(false);

    [McpTool(Cx.MarkTodoCancelCmd, Cx.MarkTodoCancelCmdDesc)]
    public static async Task<object> MarkTodoCancel(
      [Description(Cx.ItemIdParamDesc)] int itemId
    ) => await GetTool().MarkTodoCancel(new MarkTodoCancelCommand(itemId)).ConfigureAwait(false);

    #endregion

  }
}
