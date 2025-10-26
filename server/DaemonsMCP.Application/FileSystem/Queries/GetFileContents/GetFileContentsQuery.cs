using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace DaemonsMCP.Application.FileSystem.Queries.GetFileContents {
  public record GetFileContentsQuery( int projectId, int fileSystemNodeId ) : IRequest<GetFileContentsResult>;
}
