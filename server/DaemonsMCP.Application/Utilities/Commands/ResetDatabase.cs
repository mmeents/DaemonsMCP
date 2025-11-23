using DaemonsMCP.Application.FileSystem.Commands.SyncProjectFileSystem;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Application.Utilities.Commands {

  public class ResetDatabaseCommand : IRequest<ResetDatabaseResult> { }

  public class ResetDatabaseResult {
    public bool Success { get; set; }
  }

}
