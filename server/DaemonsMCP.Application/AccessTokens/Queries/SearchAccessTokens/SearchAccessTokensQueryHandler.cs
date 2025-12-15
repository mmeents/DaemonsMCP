using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DaemonsMCP.Application.AccessTokens.Queries.SearchAccessTokens {

  public record SearchAccessTokensQuery(
    string? IssuedTo = null,
    bool IncludeExpired = false,
    bool IncludeUsed = false,
    int PageNo = 1,
    int PageSize = 20
  ) : IRequest<SearchAccessTokensResult>;

  public class SearchAccessTokensQueryHandler
    : IRequestHandler<SearchAccessTokensQuery, SearchAccessTokensResult> {

    private readonly IAccessTokenRepository _repository;

    public SearchAccessTokensQueryHandler(IAccessTokenRepository repository) {
      _repository = repository;
    }

    public async Task<SearchAccessTokensResult> Handle(
      SearchAccessTokensQuery request,
      CancellationToken cancellationToken) {

      var query = _repository.GetQueryable();

      // Filter by IssuedTo
      if (!string.IsNullOrWhiteSpace(request.IssuedTo)) {
        query = query.Where(t => EF.Functions.Like(t.IssuedTo, $"%{request.IssuedTo}%"));
      }

      // Filter expired tokens
      if (!request.IncludeExpired) {
        var now = DateTime.UtcNow;
        query = query.Where(t => t.Expires > now);
      }

      // Filter used tokens
      if (!request.IncludeUsed) {
        query = query.Where(t => t.Used == null);
      }

      // Get total count before pagination
      var totalCount = await query.CountAsync(cancellationToken);

      // Apply pagination and ordering
      var tokens = await query
        .OrderByDescending(t => t.Created)
        .Skip((request.PageNo - 1) * request.PageSize)
        .Take(request.PageSize)
        .Select(t => new AccessTokenDto {
          Id = t.Id,
          ParentId = t.ParentId,
          Token = t.Token,
          IssuedTo = t.IssuedTo,
          Created = t.Created,
          Expires = t.Expires,
          Used = t.Used,
          UsedUrl = t.UsedUrl,
          UsedBy = t.UsedBy,
          UsedUrlNextToken = t.UsedUrlNextToken,
          IsValid = t.IsValid(t.UsedUrl ?? "Unused"),
          IsExpired = t.IsExpired()
        })
        .ToListAsync(cancellationToken);

      return new SearchAccessTokensResult {
        Data = tokens,
        TotalCount = totalCount,
        PageNo = request.PageNo,
        PageSize = request.PageSize
      };
    }
  }
}
