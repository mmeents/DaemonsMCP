using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Application.FileSystem.Commands.CreateProjectFile {
  public record CreateProjectFileCommand(
    int ProjectId,
    string relativePath,
    string content
  ) : IRequest<string>;
}
