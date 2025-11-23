using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Domain.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Application.FileSystem.Commands.UpdateProjectFile {
  public class UpdateProjectFileCommandHandler : 
    IRequestHandler<UpdateProjectFileCommand, UpdateProjectFileCommandResponse> {

    private readonly IFileSystemNodeRepository _fileSystemNodeRepository;
    private readonly IValidationService _validationService;
    private readonly ILogger<UpdateProjectFileCommandHandler> _logger;
    public UpdateProjectFileCommandHandler(
        IFileSystemNodeRepository fileSystemNodeRepository,
        IValidationService validationService,
        ILogger<UpdateProjectFileCommandHandler> logger
      ) {
        _fileSystemNodeRepository = fileSystemNodeRepository;
        _validationService = validationService;
        _logger = logger;
    }

    public async Task<UpdateProjectFileCommandResponse> Handle(
      UpdateProjectFileCommand request,
      CancellationToken cancellationToken) {
      var projectId = request.ProjectId;
      var fileSystemNodeId = request.FileSystemNodeId;
      var content = request.Content;
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
      return new UpdateProjectFileCommandResponse(fullPath, fileSystemNode.RelativePath);
    }


  }
}
