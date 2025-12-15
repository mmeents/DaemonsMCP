using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Models {
  public record AccessTokenDto {
    public int Id { get; init; }
    public int? ParentId { get; init; }
    public string IssuedTo { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
    public DateTime Created { get; init; }
    public DateTime Expires { get; init; }
    public DateTime? Used { get; init; }
    public string? UsedUrl { get; init; }
    public string? UsedBy { get; init; }
    public string? UsedUrlNextToken { get; init; }
    public bool IsValid { get; init; }
    public bool IsExpired { get; init; }
  }

  // Commands

  public record RevokeAccessTokenCommand(int Id) : IRequest<bool>;
  public record MarkTokenUsedCommand(string token, string UsedUrl, string UsedBy) : IRequest<AccessTokenDto>;

  // Queries
  public record GetAccessTokenByIdQuery(int Id) : IRequest<AccessTokenDto?>;
  public record GetAccessTokenByTokenQuery(string Token) : IRequest<AccessTokenDto?>;
  public record GetValidTokensQuery() : IRequest<List<AccessTokenDto>>;

}
