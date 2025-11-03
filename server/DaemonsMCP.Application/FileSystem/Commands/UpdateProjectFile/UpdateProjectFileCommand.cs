using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Application.FileSystem.Commands.UpdateProjectFile {
  public record UpdateProjectFileCommand(
    int ProjectId,
    int FileSystemNodeId,
    string Content
  ) : IRequest<UpdateProjectFileCommandResponse>;

  public record UpdateProjectFileCommandResponse(
    string FullPath,
    string RelativePath
  );

}
