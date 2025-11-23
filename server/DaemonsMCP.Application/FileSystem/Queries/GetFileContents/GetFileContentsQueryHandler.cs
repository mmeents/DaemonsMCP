using DaemonsMCP.Application.FileSystem.Commands.SyncProjectFileSystem;
using DaemonsMCP.Application.FileSystem.Services;
using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DaemonsMCP.Application.FileSystem.Queries.GetFileContents {
  public class GetFileContentsQueryHandler (
     IFileSystemNodeRepository _fileSystemNodeRepository,
     IProjectRepository _projectRepository,
     ILogger<GetFileContentsQueryHandler> _logger
    ) 
    : IRequestHandler<GetFileContentsQuery, GetFileContentsResult> {


    public async Task<GetFileContentsResult> Handle(GetFileContentsQuery request, CancellationToken cancellationToken) {

      // 1. Get the FileSystemNode
      var node = await _fileSystemNodeRepository.GetByIdAsync(request.fileSystemNodeId, cancellationToken).ConfigureAwait(false);
      if (node == null || node.ProjectId != request.projectId) {
        _logger.LogWarning("fileSystemNode not there or project mismatch");
        throw new Exception($"File {request.fileSystemNodeId} not found in project {request.projectId}");
      }

      if (node.IsDirectory) {
        throw new InvalidOperationException("Cannot read directory as file");
      }

      // 2. Get the project for RootPath
      var project = await _projectRepository.GetByIdAsync(request.projectId, cancellationToken).ConfigureAwait(false);
      if (project == null) {
        _logger.LogWarning("Project not found via lookup");
        throw new Exception($"Project {request.projectId} not found");
      }

      // 3. Build full path
      var fullPath = Path.Combine(project.RootPath, node.RelativePath);

      // 4. Security check (paranoid but good)
      if (!fullPath.StartsWith(project.RootPath, StringComparison.OrdinalIgnoreCase)) {
        _logger.LogWarning("Dir check traversal error.");
        throw new SecurityException("Path traversal attempt detected");
      }

      // 5. Read file
      if (!File.Exists(fullPath)) {
        _logger.LogWarning($"file not found {fullPath}");
        throw new FileNotFoundException($"Physical file not found: {fullPath}");
      }

      var content = await File.ReadAllTextAsync(fullPath, cancellationToken).ConfigureAwait(false);

      return new GetFileContentsResult {
        FileSystemNodeId = node.Id,
        ProjectId = request.projectId,        
        RelativePath = node.RelativePath,
        Content = content,
        SizeInBytes = node.SizeInBytes
      };
    }
  }
}
