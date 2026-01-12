using DaemonsMCP.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DaemonsMCP.Domain.Models;

namespace DaemonsMCP.Application.Invitations.Queries.SearchInvitationTokens {

  public record SearchInvitationTokensQuery(
    string? InvitedEmail = null,
    bool IncludeExpired = false,
    bool IncludeUsed = false,
    int PageNo = 1,
    int PageSize = 20
  ) : IRequest<SearchInvitationTokensResult>;

  public class SearchInvitationTokensQueryHandler : IRequestHandler<SearchInvitationTokensQuery, SearchInvitationTokensResult> {
    private readonly IInvitationTokenRepository _repository;
    public SearchInvitationTokensQueryHandler(IInvitationTokenRepository repository) {
      _repository = repository;
    }
    public async Task<SearchInvitationTokensResult> Handle(
      SearchInvitationTokensQuery request, 
      CancellationToken cancellationToken) 
    {
      var query = _repository.GetQueryable();

      if (!string.IsNullOrEmpty(request.InvitedEmail)) {
        query = query.Where(t => EF.Functions.Like( t.InvitedEmail, $"%{request.InvitedEmail}%" ));
      }

      if (!request.IncludeExpired) {
        query = query.Where(t => t.ExpiresAt > DateTime.UtcNow);
      }

      if (!request.IncludeUsed) {
        query = query.Where(t => !t.IsUsed);
      }

      var totalCount = await query.CountAsync(cancellationToken);
      var items = await query
        .Skip((request.PageNo - 1) * request.PageSize)
        .Take(request.PageSize)
        .ToListAsync(cancellationToken);

      return new SearchInvitationTokensResult {
        Data = items.Select(t => new InvitationTokenDto {
          Id = t.Id,
          InvitedEmail = t.InvitedEmail,
          ExpiresAt = t.ExpiresAt,
          IsUsed = t.IsUsed
        }).ToList(),
        TotalCount = totalCount,
        PageNo = request.PageNo,
        PageSize = request.PageSize
      };
    }
  }

    
}
