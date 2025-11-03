using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaemonsMCP.Domain.Constants;
using MCPSharp;
using DaemonsMCP.Domain.Extensions;

namespace DaemonsMCP.Infrastructure.Tools {
  public class ItemTypeTools {
    private static IItemTypeToolsHandler GetTool() => DIServiceBridge.GetService<IItemTypeToolsHandler>();

    [McpTool(Cx.GetStatusTypesCmd, Cx.GetStatusTypesCmdDesc)]
    public static async Task<string> GetStatusTypes() {
      return await GetTool().GetStatusTypes();
    }

    [McpTool(Cx.GetItemTypesCmd, Cx.GetItemTypesCmdDesc)]
    public static async Task<string> GetItemTypes() {
      return await GetTool().GetItemTypes();
    }
  }
}
