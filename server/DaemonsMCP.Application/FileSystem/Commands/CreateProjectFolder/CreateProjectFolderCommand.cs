using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace DaemonsMCP.Application.FileSystem.Commands.CreateProjectFolder {
  public record CreateProjectFolderCommand (
    int ProjectId, string RelativePath
    ) : IRequest<string> ;
}
