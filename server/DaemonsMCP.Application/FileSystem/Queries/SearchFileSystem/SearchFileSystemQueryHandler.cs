using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DaemonsMCP.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DaemonsMCP.Application.FileSystem.Queries.SearchFileSystem{

  public class SearchFileSystemQueryHandler
      : IRequestHandler<SearchFileSystemQuery, SearchFileSystemResult> {
    private readonly IFileSystemNodeRepository _repository;

    public SearchFileSystemQueryHandler(IFileSystemNodeRepository repository) {
      _repository = repository;
    }

    public async Task<SearchFileSystemResult> Handle(
        SearchFileSystemQuery request,
        CancellationToken cancellationToken) {
      // Start with base query for the project
      var query = _repository.GetQueryable()
          .Where(f => f.ProjectId == request.ProjectId);

      // Filter by file/directory type
      if (!request.IncludeDirectories || !request.IncludeFiles) {
        query = query.Where(f =>
            (request.IncludeDirectories && f.IsDirectory) ||
            (request.IncludeFiles && !f.IsDirectory));
      }

      // Apply text filter on Name or RelativePath
      if (!string.IsNullOrWhiteSpace(request.Filter)) {
        var filter = request.Filter;
        query = query.Where(f =>
            f.Name.Contains(filter) ||
            f.RelativePath.Contains(filter));
      }

      // Get total count before pagination
      var totalCount = await query.CountAsync(cancellationToken);

      // Apply pagination and select
      var results = await query
          .OrderBy(f => f.RelativePath)
          .Skip((request.PageNo - 1) * request.PageSize)
          .Take(request.PageSize)
          .Select(f => new FileSystemNodeDto {
            Id = f.Id,
            ParentId = f.ParentId,
            Name = f.Name,
            RelativePath = f.RelativePath,
            IsDirectory = f.IsDirectory,
            SizeInBytes = f.SizeInBytes
          })
          .ToListAsync(cancellationToken);

      return new SearchFileSystemResult {
        Data = results,
        TotalCount = totalCount,
        PageNo = request.PageNo,
        PageSize = request.PageSize
      };
    }
  }

}
