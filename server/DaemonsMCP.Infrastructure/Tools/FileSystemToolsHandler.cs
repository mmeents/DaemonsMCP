using DaemonsMCP.Application.FileSystem.Queries.GetFileContents;
using DaemonsMCP.Application.FileSystem.Queries.SearchFileSystem;
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
        // Create a scope to resolve scoped services like IMediator and repositories
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
        // Create a scope to resolve scoped services like IMediator and repositories
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
      string path = relativePath;
      try {
        using var scope = _scopeFactory.CreateScope();
        var _validationService = scope.ServiceProvider.GetRequiredService<IValidationService>();
        var _fileSystemNodeRepository = scope.ServiceProvider.GetRequiredService<IFileSystemNodeRepository>();

        _validationService.ValidatePath(path);
        _validationService.ValidateContent(content);
        FileValidationContext context = await _validationService.ValidateAndPrepareFile(projectId, path, true);
        var fullPath = context.FullPath;        

        // Create directory if needed
        var directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) {
          Directory.CreateDirectory(directory);
        }

        if (File.Exists(fullPath)) {
          context.Project.CopyToBackup(fullPath);
          _logger.LogInformation("Backup created for file being overwritten: {FilePath}", fullPath);
        }

        // Write the file
        await File.WriteAllTextAsync(fullPath, content, Encoding.UTF8);

        // Return success info
        var fileInfo = new FileInfo(fullPath);

        _ = await _fileSystemNodeRepository.GetOrCreateAsync(
          projectId,
          context.RelativePath,
          isDirectory: false,
          fileSizeBytes: fileInfo.Length
        );        

        _logger.LogInformation("File created successfully: {FilePath}", fullPath);
        var opResult = McpOpResult.CreateSuccess(
          Cx.InsertFileCmd,
          $"File created successfully: {path}",
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
        var _validationService = scope.ServiceProvider.GetRequiredService<IValidationService>();
        var _fileSystemNodeRepository = scope.ServiceProvider.GetRequiredService<IFileSystemNodeRepository>();

        var fileSystemNode = await _fileSystemNodeRepository.GetByIdAsync(fileSystemNodeId);
        if (fileSystemNode == null || fileSystemNode.ProjectId != projectId || fileSystemNode.IsDirectory) {
          throw new FileNotFoundException("File not found for the given FileSystemNodeId and ProjectId");
        }

        // Validate inputs
        FileValidationContext context = await _validationService.ValidateAndPrepareFile(projectId, fileSystemNode.RelativePath, false);
        var fullPath = context.FullPath;
        var path = fileSystemNode.RelativePath;
        _validationService.ValidatePath(fullPath);
        _validationService.ValidateContent(content);
        
        // Security validations
        if (!_validationService.IsWriteAllowed(fullPath, context.Filters)) {
          throw new UnauthorizedAccessException("Write operation not allowed for security reasons");
        }       

        string backupPath = context.Project.CopyToBackup(fullPath);
        _logger.LogInformation("Backup created for file being overwritten: {FilePath}", fullPath);

        // UpdateClassItem the file
        await File.WriteAllTextAsync(fullPath, content, Encoding.UTF8).ConfigureAwait(false);

        // Return success info
        var fileInfo = new FileInfo(fullPath);
        var relativePath = path;        

        var opResult = McpOpResult.CreateSuccess(
          Cx.UpdateFileCmd,
          $"File updated successfully: {path}",
          new {
            fileName = fileInfo.Name,
            path = relativePath,
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
        var _validationService = scope.ServiceProvider.GetRequiredService<IValidationService>();
        var _fileSystemNodeRepository = scope.ServiceProvider.GetRequiredService<IFileSystemNodeRepository>();

        var context = await _validationService.ValidateAndPrepareFolder(projectId, path, true);
        var fullDirPath = context.FullPath;

        // In CreateProjectFile
        var directory = Path.GetDirectoryName(fullDirPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) {
          Directory.CreateDirectory(directory);
        }
        await _fileSystemNodeRepository.GetOrCreateAsync(projectId, context.RelativePath, isDirectory: true, fileSizeBytes: 0 );

        // Return success info
        var dirInfo = new DirectoryInfo(fullDirPath);        
        var opResult = McpOpResult.CreateSuccess(
          Cx.CreateFolderCmd,
          $"Directory created successfully: {path}",
          new {
            directoryName = dirInfo.Name,
            path = context.RelativePath,
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

  
