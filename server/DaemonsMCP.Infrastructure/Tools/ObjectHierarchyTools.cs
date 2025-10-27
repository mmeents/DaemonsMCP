using DaemonsMCP.Domain.Constants;
using DaemonsMCP.Infrastructure.Extensions;
using DaemonsMCP.Domain.Models;
using MCPSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace DaemonsMCP.Infrastructure.Tools {
  public class ObjectHierarchyTools {
    private static IObjectHierarchyToolsHandler GetTool() => DIServiceBridge.GetService<IObjectHierarchyToolsHandler>();

    [McpTool(Cx.SearchObjectHierarchyCmd, Cx.SearchObjectHierarchyDesc)]
    public static async Task<string> ListObjectHierarchy(
      [Description(Cx.ProjectParamDesc)] int projectId,
      [Description(Cx.PageNoParamDesc)] int PageNo,
      [Description(Cx.ItemsPerPageParamDesc)] int PageSize,
      [Description(Cx.SearchTermParamDesc)] string? SearchTerm,
      [Description(Cx.IdentifierTypeFilterParamDesc)] int? IdentifierTypeId,
      [Description(Cx.FileSystemNodeIdParamDesc)] int? FileSystemNodeId,
      [Description(Cx.ParentIdFilterParamDesc)] int? ParentId
    ) {
      var request = new SearchObjectHierarchyQuery(
        projectId,        
        SearchTerm,
        IdentifierTypeId,
        FileSystemNodeId,
        ParentId,
        PageNo,
        PageSize
      );
      return await GetTool().SearchObjectHierarchy(request);
    }
  }
}
