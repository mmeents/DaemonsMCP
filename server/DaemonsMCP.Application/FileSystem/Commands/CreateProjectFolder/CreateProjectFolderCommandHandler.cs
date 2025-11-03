using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Application.FileSystem.Commands.CreateProjectFolder {
  public class CreateProjectFolderCommandHandler :
    IRequestHandler<CreateProjectFolderCommand, string> 
  {
    private readonly IValidationService _validationService;
    private readonly IFileSystemNodeRepository _fileSystemNodeRepository;
    private readonly ILogger<CreateProjectFolderCommandHandler> _logger;
    public CreateProjectFolderCommandHandler(
      IFileSystemNodeRepository fileSystemNodeRepository,
      IValidationService validationService,
      ILogger<CreateProjectFolderCommandHandler> logger) {
      _fileSystemNodeRepository = fileSystemNodeRepository;
      _validationService = validationService;
      _logger = logger;
    }

    public async Task<string> Handle(CreateProjectFolderCommand request, CancellationToken cancellationToken) {
      var projectId = request.ProjectId;
      var path = request.RelativePath;

      var context = await _validationService.ValidateAndPrepareFolder(projectId, path, true);
      var fullDirPath = context.FullPath;

      // In CreateProjectFile
      var directory = Path.GetDirectoryName(fullDirPath);
      if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) {
        Directory.CreateDirectory(directory);
      }
      await _fileSystemNodeRepository.GetOrCreateAsync(projectId, context.RelativePath, isDirectory: true, fileSizeBytes: 0);

      return fullDirPath;

    }
  }
}
