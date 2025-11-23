using DaemonsMCP.Application.FileSystem.Queries.GetFileContents;
using DaemonsMCP.Application.FileSystem.Queries.SearchFileSystem;
using DaemonsMCP.Application.FileSystem.Commands.CreateProjectFile;
using DaemonsMCP.Application.FileSystem.Commands.UpdateProjectFile;
using DaemonsMCP.Application.FileSystem.Commands.CreateProjectFolder;
using DaemonsMCP.Domain.Constants;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Extensions;
using DaemonsMCP.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using DaemonsMCP.Domain.Repositories;

namespace DaemonsMCP.Infrastructure.Tools {
  public class FileSystemToolsHandler: IFileSystemToolsHandler {
    private ILogger<FileSystemToolsHandler> _logger;
    private IServiceScopeFactory _scopeFactory;

    public FileSystemToolsHandler(
      ILogger<FileSystemToolsHandler> logger,            
      IServiceScopeFactory scopeFactory
    ) 
    {
      _logger = logger;
      _scopeFactory = scopeFactory;      
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

        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();         
        
        var query = new SearchFileSystemQuery(
          projectId,
          filter,
          includeDirectories,
          includeFiles,
          pageNo,
          pageSize);
        var result = await mediator.Send(query);
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
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var query = new GetFileContentsQuery(projectId, fileSystemNodeId);
        var result = await mediator.Send(query);
        var opResult = McpOpResult.CreateSuccess(Cx.GetFileCmd, $"{Cx.GetFileCmd} Success.", result);
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error Getting File");
        var opResult = McpOpResult.CreateFailure(Cx.GetFileCmd, $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<string> CreateProjectFile( int projectId, string relativePath, string content) {     
      try {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var query = new CreateProjectFileCommand(
          projectId,
          relativePath,
          content
        );
        var filePath = await mediator.Send(query);
        var fileInfo = new FileInfo(filePath);
        _logger.LogInformation("File created successfully: {FilePath}", filePath);
        var opResult = McpOpResult.CreateSuccess(
          Cx.InsertFileCmd,
          $"File created successfully: {filePath}",
          new {
            fileName = fileInfo.Name,
            path = relativePath,
            size = fileInfo.Length,
            created = fileInfo.CreationTime,
          }
        );
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, $"Error creating file: {relativePath} in project: {projectId}");
        var opResult = McpOpResult.CreateFailure(Cx.InsertFileCmd, $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<string> UpdateProjectFile( int projectId, int fileSystemNodeId, string content) {
      try {
        using var scope = _scopeFactory.CreateScope();
        var _meditor = scope.ServiceProvider.GetRequiredService<IMediator>();
        var query = new UpdateProjectFileCommand(
          projectId,
          fileSystemNodeId,
          content
        );
        var result = await _meditor.Send(query);
        // Return success info
        var fileInfo = new FileInfo(result.FullPath);        
        var opResult = McpOpResult.CreateSuccess(
          Cx.UpdateFileCmd,
          $"File updated successfully: {result.FullPath}",
          new {
            fileName = fileInfo.Name,
            path = result.RelativePath,
            size = fileInfo.Length,
            modified = fileInfo.LastWriteTime,
            backupCreated = true            
          }
        );        
        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, "Error Updating File");
        var opResult = McpOpResult.CreateFailure(Cx.UpdateFileCmd, $"Failed: {ex.Message}", null);
        return JsonSerializer.Serialize(opResult);
      }
    }

    public async Task<string> CreateFolder(int projectId, string path) {
      try {
        using var scope = _scopeFactory.CreateScope();
        var _mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var query = new CreateProjectFolderCommand(
          projectId,
          path
        );  
        var fullDirPath = await _mediator.Send(query);
        // Return success info
        var dirInfo = new DirectoryInfo(fullDirPath);        
        var opResult = McpOpResult.CreateSuccess(
          Cx.CreateFolderCmd,
          $"Directory created successfully: {path}",
          new {
            directoryName = dirInfo.Name,
            path = path,
            created = dirInfo.CreationTime,
          }
        );

        return JsonSerializer.Serialize(opResult);
      } catch (Exception ex) {
        _logger.LogError(ex, $"Error creating directory: {path} in projectId: {projectId}");
        var opResult = McpOpResult.CreateFailure(Cx.CreateFolderCmd, $"Error creating directory: {path} in projectId: {projectId}: {ex.Message}", null);
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

    public Task<string> CreateProjectFile(int projectId, string relativePath, string content);

    public Task<string> UpdateProjectFile(int projectId, int fileSystemNodeId, string content);

    public Task<string> CreateFolder(int projectId, string path);
  }

}

  
