using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Extensions;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging; 

namespace DaemonsMCP.Application.AccessTokens.Commands.MarkTokenUsed {
  public class MarkTokenUsedCommandHandler : IRequestHandler<MarkTokenUsedCommand, AccessTokenDto> {
    private readonly IAccessTokenRepository _repository;
    private readonly ILogger<MarkTokenUsedCommandHandler> _logger;


    public MarkTokenUsedCommandHandler(IAccessTokenRepository repository, ILogger<MarkTokenUsedCommandHandler> logger) {
      _repository = repository;
      _logger = logger;
    }
    
    public async Task<AccessTokenDto> Handle(MarkTokenUsedCommand request, CancellationToken cancellationToken) {

      var token = await _repository.GetByTokenAsync(request.token, cancellationToken);

      if (token == null) {
        throw new UnauthorizedAccessException($"Token with value {request.token} not found");
      }
      if (!token.IsValid(request.UsedUrl)) {
        throw new UnauthorizedAccessException("Token is already used or expired");
      }

      string? nextTokenValue = null;
      if (token.UsedUrlNextToken != null) {
        nextTokenValue = token.UsedUrlNextToken;
        var nextToken = await _repository.GetByTokenAsync(nextTokenValue, cancellationToken);

        if (nextToken == null) {
          throw new UnauthorizedAccessException($"Next token with value {nextTokenValue} not found");
        }
        return new AccessTokenDto {
          Id = nextToken.Id,
          ParentId = nextToken.ParentId,
          Token = nextToken.Token,
          Created = nextToken.Created,
          Expires = nextToken.Expires,
          IsValid = nextToken.IsValid("Unused"),
          IsExpired = nextToken.IsExpired(),
          UsedUrlNextToken = nextToken.UsedUrlNextToken
        };
      }

      using var transaction = await _repository.BeginTransactionAsync(cancellationToken);
      try { 
        
        var nextTokenValue2 = await _repository.FindUnusedNewToken(cancellationToken);
        token.MarkAsUsed(request.UsedUrl, request.UsedBy, nextTokenValue2);
        await _repository.UpdateAsync(token, cancellationToken);
        

        var nextToken = new AccessToken(nextTokenValue2, token.IssuedTo, DateTime.UtcNow.AddMinutes(60), token.ParentId==null ? token.Id: token.ParentId);
        nextToken = await _repository.AddAsync(nextToken, cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new AccessTokenDto {        
          Id = nextToken.Id,
          ParentId = nextToken.ParentId,
          Token = nextToken.Token,
          Created = nextToken.Created,
          Expires = nextToken.Expires,
          IsValid = nextToken.IsValid("Unused"),
          IsExpired = nextToken.IsExpired()
        };

      } catch (Exception ex) {
        _logger.LogError(ex, "Error marking token used for token {Token}", request.token);
        await transaction.RollbackAsync(cancellationToken);
        throw;
      }

    }
  }
}
