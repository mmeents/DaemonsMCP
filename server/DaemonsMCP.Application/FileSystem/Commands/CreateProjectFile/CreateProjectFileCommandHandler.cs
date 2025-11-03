using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Domain.Extensions;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DaemonsMCP.Application.Projects.Commands.CreateProject;


namespace DaemonsMCP.Application.FileSystem.Commands.CreateProjectFile {

  public class CreateProjectFileCommandHandler : IRequestHandler<CreateProjectFileCommand, string> { 
    
    private readonly IFileSystemNodeRepository _fileSystemRepository;
    private readonly IValidationService _validationService;
    private readonly ILogger<CreateProjectFileCommandHandler> _logger;
    public CreateProjectFileCommandHandler(
      IFileSystemNodeRepository fileSystemRepository,
      IValidationService validationService,
      ILogger<CreateProjectFileCommandHandler> logger
      ) {
      _fileSystemRepository = fileSystemRepository;
      _validationService = validationService;
      _logger = logger;

    }

    public async Task<string> Handle(CreateProjectFileCommand request, CancellationToken cancellationToken) {

      var projectId = request.ProjectId;
      var path = request.relativePath;
      var content = request.content;
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

      _ = await _fileSystemRepository.GetOrCreateAsync(
        projectId,
        context.RelativePath,
        isDirectory: false,
        fileSizeBytes: fileInfo.Length
      );

      return fullPath;

    }
  }

}
