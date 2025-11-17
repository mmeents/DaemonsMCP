using DaemonsMCP.Application.FileSystem.Commands.SyncProjectFileSystem;
using DaemonsMCP.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Application.Utilities.Commands {
  public class ResetDatabaseCommandHandler : IRequestHandler<ResetDatabaseCommand, ResetDatabaseResult> {
    private readonly IDatabaseManagementService _databaseService;
    private readonly IMediator _mediator;

    public async Task<ResetDatabaseResult> Handle(ResetDatabaseCommand request, CancellationToken cancellationToken) {
      // Reset database to be used with a manual restart.
      // This command drops the index and resets the Identity counters.
      await _databaseService.ResetDatabaseAsync(cancellationToken);

      return new ResetDatabaseResult { Success = true };
    }
  }

}
