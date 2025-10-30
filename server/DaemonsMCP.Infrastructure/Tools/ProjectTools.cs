using DaemonsMCP.Domain.Constants;
using DaemonsMCP.Domain.Extensions;
using MCPSharp;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DaemonsMCP.Application.Projects.Queries.GetAllProjects;
using DaemonsMCP.Domain.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Text.Json;

namespace DaemonsMCP.Infrastructure.Tools {
  public class ProjectTools {

    private static IProjectToolsHandler GetTools() => DIServiceBridge.GetService<IProjectToolsHandler>();

    [McpTool(Cx.ListProjectsCmd, Cx.ListProjectsDesc)]
    public static async Task<string> ListProjects() => await GetTools().ListProjectsAsync();

  }
}
