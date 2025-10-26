using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using DaemonsMCP.Application.FileSystem.Services;

namespace DaemonsMCP.Application.FileSystem.Commands.SyncProjectFileSystem;

public record SyncProjectFileSystemCommand(
    int ProjectId
) : IRequest<SyncResult>;
