using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using MediatR;

namespace DaemonsMCP.Application.AccessTokens.Queries.GetAccessTokenById {
  public class GetAccessTokenByIdQueryHandler
    : IRequestHandler<GetAccessTokenByIdQuery, AccessTokenDto?> {

    private readonly IAccessTokenRepository _repository;

    public GetAccessTokenByIdQueryHandler(IAccessTokenRepository repository) {
      _repository = repository;
    }

    public async Task<AccessTokenDto?> Handle(
      GetAccessTokenByIdQuery request,
      CancellationToken cancellationToken) {

      var token = await _repository.GetByIdAsync(request.Id, cancellationToken);

      if (token == null) {
        return null;
      }

      return new AccessTokenDto {
        Id = token.Id,
        ParentId = token.ParentId,
        Token = token.Token,
        IssuedTo = token.IssuedTo,
        Created = token.Created,
        Expires = token.Expires,
        Used = token.Used,
        UsedUrl = token.UsedUrl,
        UsedBy = token.UsedBy,
        UsedUrlNextToken = token.UsedUrlNextToken,
        IsValid = token.IsValid(token.UsedUrl ?? "Unused"),
        IsExpired = token.IsExpired()
      };
    }
  }
}