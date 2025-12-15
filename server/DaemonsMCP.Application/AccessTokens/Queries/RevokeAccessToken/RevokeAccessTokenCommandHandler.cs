using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DaemonsMCP.Application.AccessTokens.Commands.RevokeAccessToken {
  public class RevokeAccessTokenCommandHandler
    : IRequestHandler<RevokeAccessTokenCommand, bool> {

    private readonly IAccessTokenRepository _repository;
    private readonly ILogger<RevokeAccessTokenCommandHandler> _logger;

    public RevokeAccessTokenCommandHandler(
      IAccessTokenRepository repository,
      ILogger<RevokeAccessTokenCommandHandler> logger) {
      _repository = repository;
      _logger = logger;
    }

    public async Task<bool> Handle(
      RevokeAccessTokenCommand request,
      CancellationToken cancellationToken) {

      var token = await _repository.GetByIdAsync(request.Id, cancellationToken);

      if (token == null) {
        _logger.LogWarning("Revoke failed: Token {TokenId} not found", request.Id);
        return false;
      }

      // Find the root parent token to revoke the entire chain
      var rootTokenId = token.ParentId ?? token.Id;
      var rootToken = await _repository.GetByIdAsync(rootTokenId, cancellationToken);

      if (rootToken == null) {
        _logger.LogWarning("Revoke failed: Root token {RootTokenId} not found", rootTokenId);
        return false;
      }

      // Mark the root token chain as revoked
      rootToken.IsRevokedChain = true;
      await _repository.UpdateAsync(rootToken, cancellationToken);
      await _repository.SaveChangesAsync(cancellationToken);

      _logger.LogInformation(
        "Token chain revoked: TokenId {TokenId}, RootTokenId {RootTokenId}",
        request.Id,
        rootTokenId);

      return true;
    }
  }
}
