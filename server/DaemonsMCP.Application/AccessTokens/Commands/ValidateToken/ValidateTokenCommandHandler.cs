using DaemonsMCP.Application.AccessTokens.Commands.MarkTokenUsed;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Application.AccessTokens.Commands.ValidateToken {
  public class ValidateTokenCommandHandler : IRequestHandler<ValidateTokenCommand, AccessTokenDto> {
    private readonly IAccessTokenRepository _repository;
    private readonly ILogger<ValidateTokenCommandHandler> _logger;

    public ValidateTokenCommandHandler(IAccessTokenRepository repository, ILogger<ValidateTokenCommandHandler> logger) {
      _repository = repository;
      _logger = logger;
    }

    public async Task<AccessTokenDto> Handle(ValidateTokenCommand request, CancellationToken cancellationToken) {
      var token = request.token;
      
      if (string.IsNullOrWhiteSpace(token)) {
        _logger.LogWarning("Token validation failed: Empty token provided");
        throw new UnauthorizedAccessException("Token cannot be empty");
      }
      
      var accessToken = await _repository.GetByTokenAsync(token, cancellationToken);
      
      // DETAILED FAILURE LOGGING - exactly what's wrong
      if (accessToken == null) {
        _logger.LogWarning("Token validation failed: Token not found in database - Token: {Token}", token);
        throw new UnauthorizedAccessException("Token not found");
      }
      
      if (accessToken.IsExpired()) {
        _logger.LogWarning("Token validation failed: Token expired - Token: {Token}, Expired: {ExpiryTime}, Now: {CurrentTime}", 
          token, 
          accessToken.Expires.ToString("yyyy-MM-dd HH:mm:ss.fff"), 
          DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        throw new UnauthorizedAccessException("Token expired");
      }
      
      if (accessToken.UsedUrl != null && accessToken.UsedUrl != request.usedUrl) {
        _logger.LogWarning("Token validation failed: Token already used - Token: {Token}, UsedBy: {UsedBy}, UsedUrl: {UsedUrl}", 
          token,           
          accessToken.UsedBy ?? "unknown", 
          accessToken.UsedUrl ?? "unknown");
        throw new UnauthorizedAccessException("Token already used");
      }
      
      if (accessToken.Parent?.IsRevokedChain ?? false) {
        _logger.LogWarning("Token validation failed: Token chain revoked - Token: {Token}, ParentId: {ParentId}", 
          token, 
          accessToken.ParentId);
        throw new UnauthorizedAccessException("Token chain revoked");
      }
      
      // Token is valid - log success
      _logger.LogInformation("Token validated successfully - Token: {Token}, Created: {Created}, Expires: {Expires}", 
        token,
        accessToken.Created.ToString("yyyy-MM-dd HH:mm:ss.fff"),
        accessToken.Expires.ToString("yyyy-MM-dd HH:mm:ss.fff"));
      
      return new AccessTokenDto {
        Id = accessToken.Id,
        ParentId = accessToken.ParentId,
        Token = accessToken.Token,
        Created = accessToken.Created,
        Expires = accessToken.Expires,
        IsValid = accessToken.IsValid(request.usedUrl),
        IsExpired = accessToken.IsExpired()
      };
    }
  }

  public record ValidateTokenCommand(string token, string usedUrl) : IRequest<AccessTokenDto>;


}
