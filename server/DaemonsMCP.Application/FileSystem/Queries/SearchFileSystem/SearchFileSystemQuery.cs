using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace DaemonsMCP.Application.FileSystem.Queries.SearchFileSystem {  

  public record SearchFileSystemQuery(
      int ProjectId,
      string? Filter = null,
      bool IncludeDirectories = true,
      bool IncludeFiles = true,
      int PageNo = 1,
      int PageSize = 20
  ) : IRequest<SearchFileSystemResult>;


}
