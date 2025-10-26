using Azure;
using DaemonsMCP.Application.FileSystem.Queries.SearchFileSystem;
using DaemonsMCP.Application.FileSystem.Queries.GetFileContents;
using DaemonsMCP.Domain.Constants;
using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Models;
using MediatR;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace DaemonsMCP.Infrastructure.Tools {
  public class FileSystemToolsHandler: IFileSystemToolsHandler {
    private ILogger<FileSystemToolsHandler> _logger;
    private IMediator _mediator;
    public FileSystemToolsHandler(
      ILogger<FileSystemToolsHandler> logger,
      IMediator mediator
    ) 
    {
      _logger = logger;
      _mediator = mediator;
    }

    public async Task<string> SearchFileSystem(
        int projectId,
        string? filter,
        bool includeDirectories = true,
        bool includeFiles = true,
        int pageNo = 1,
        int pageSize = 20 ) 
    {

      try { 
        var query = new SearchFileSystemQuery(
          projectId,
          filter,
          includeDirectories,
          includeFiles,
          pageNo,
          pageSize);
        var result = await _mediator.Send(query);
        var opResult = McpOpResult.CreateSuccess(Cx.ListFileSystemCmd, $"{Cx.ListFileSystemCmd} Success.", result);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error Searching File System");
        var opResult = McpOpResult.CreateFailure(Cx.ListFileSystemCmd, $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }

    }

    public async Task<string> GetFile(int projectId, int fileSystemNodeId) {
      try {
        var query = new GetFileContentsQuery(projectId, fileSystemNodeId);
        var result = await _mediator.Send(query);
        var opResult = McpOpResult.CreateSuccess(Cx.GetFileCmd, $"{Cx.GetFileCmd} Success.", result);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error Getting File");
        var opResult = McpOpResult.CreateFailure(Cx.GetFileCmd, $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }


  }


  public interface IFileSystemToolsHandler {
    public Task<string> SearchFileSystem(
      int projectId,
      string? filter,
      bool includeDirectories = true,
      bool includeFiles = true,
      int pageNo = 1,
      int pageSize = 20
    );

    public Task<string> GetFile(int projectId, int fileSystemNodeId);
  }

}

  
