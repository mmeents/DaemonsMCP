using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using DaemonsMCP.Application.FileSystem.Services;
using DaemonsMCP.Domain.Repositories;

namespace DaemonsMCP.Application.FileSystem.Commands.SyncProjectFileSystem;

public class SyncProjectFileSystemCommandHandler
    : IRequestHandler<SyncProjectFileSystemCommand, SyncResult> {
  private readonly IProjectRepository _projectRepository;
  private readonly IFileSystemSyncService _syncService;

  public SyncProjectFileSystemCommandHandler(
      IProjectRepository projectRepository,
      IFileSystemSyncService syncService) {
    _projectRepository = projectRepository;
    _syncService = syncService;
  }

  public async Task<SyncResult> Handle(
      SyncProjectFileSystemCommand request,
      CancellationToken cancellationToken) {
    // Get project to verify it exists and get root path
    var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);

    if (project == null) {
      throw new InvalidOperationException($"Project with ID {request.ProjectId} not found.");
    }

    // Delegate to sync service
    return await _syncService.SyncProjectAsync(
        project,
        cancellationToken);
  }
}