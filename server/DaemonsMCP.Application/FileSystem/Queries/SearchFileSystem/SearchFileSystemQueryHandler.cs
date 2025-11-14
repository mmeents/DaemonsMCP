using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DaemonsMCP.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DaemonsMCP.Domain.Extensions;

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

      var query = _repository.GetQueryable()
          .Where(f => f.ProjectId == request.ProjectId);

      // Filter by file/directory type
      if (!request.IncludeDirectories || !request.IncludeFiles) {
        query = query.Where(f =>
            (request.IncludeDirectories && f.IsDirectory) ||
            (request.IncludeFiles && !f.IsDirectory));
      }

      // Apply smart search filter at database level
      if (!string.IsNullOrWhiteSpace(request.Filter)) {
        var searchTerms = GetSearchTerms(request.Filter);

        // Build the WHERE clause for database filtering
        // We'll do simple contains for now and score later
        foreach (var term in searchTerms) {
          var termCopy = term; // Avoid closure issues
          query = query.Where(f =>
              EF.Functions.Like(f.Name, $"%{termCopy}%"));
        }
      }

      // Get total count before pagination
      var totalCount = await query.CountAsync(cancellationToken);

      // Apply initial ordering and pagination at database level
      var dbResults = await query
          .OrderBy(f => f.RelativePath)
          .Skip((request.PageNo - 1) * request.PageSize)
          .Take(request.PageSize)
          .Select(f => new FileSystemNodeDto {
            Id = f.Id,
            ParentId = f.ParentId,
            ProjectId = f.ProjectId,
            Name = f.Name,
            RelativePath = f.RelativePath,
            IsDirectory = f.IsDirectory,
            SizeInBytes = f.SizeInBytes
          })
          .ToListAsync(cancellationToken); // Execute database query here

      // Now apply scoring in memory if we have a filter
      List<FileSystemNodeDto> finalResults;
      if (!string.IsNullOrWhiteSpace(request.Filter)) {
        finalResults = dbResults
            .Select(item => new {
              Item = item,
              Score = CalculateRelevanceScore(item.Name, request.Filter)
            })
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.Item.RelativePath)
            .Select(x => x.Item)
            .ToList();
      } else {
        finalResults = dbResults;
      }

      return new SearchFileSystemResult {
        Data = finalResults,
        Filter = request.Filter ?? string.Empty,
        TotalCount = totalCount,
        PageNo = request.PageNo,
        PageSize = request.PageSize
      };
    }

    private double CalculateRelevanceScore(string fileName, string searchTerm) {
      var score = 0.0;
      var lowerFile = fileName.ToLower();
      var lowerSearch = searchTerm.ToLower();

      // Exact match
      if (lowerFile == lowerSearch) return 100.0;

      // Starts with full search term
      if (lowerFile.StartsWith(lowerSearch)) score += 50.0;

      // Contains full search term
      if (lowerFile.Contains(lowerSearch)) score += 30.0;

      // Check how many search parts match
      var searchParts = GetSearchTerms(searchTerm);
      var fileParts = GetSearchTerms(fileName);

      var matchingParts = searchParts.Count(sp =>
          fileParts.Any(fp => fp.Contains(sp) || sp.Contains(fp)));

      score += (matchingParts / (double)searchParts.Count) * 20.0;

      return score;
    }

    private List<string> GetSearchTerms(string filter) {
      var terms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);      
      var parts = filter.Split(new[] { ' ', '_', '-', '.', '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
      foreach (var part in parts) {        
        terms.Add(part.ToLower());                
        var camelParts = part.SplitCamelCase();        
        foreach (var cp in camelParts) {
          if (cp.Length > 1) { // Skip single characters
            terms.Add(cp);
          }
        }
      }
      return terms.ToList();
    }

  }

}
