using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Extensions;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using MediatR;

namespace DaemonsMCP.Application.AccessTokens.Commands.CreateAccessToken;

public class CreateAccessTokenCommandHandler : IRequestHandler<CreateAccessTokenCommand, AccessTokenDto> {
  private readonly IAccessTokenRepository _repository;

  public CreateAccessTokenCommandHandler(IAccessTokenRepository repository) {
    _repository = repository;
  }

  public async Task<AccessTokenDto> Handle(CreateAccessTokenCommand request, CancellationToken cancellationToken) {
    // Validate parent exists if provided
    if (request.ParentId.HasValue) {
      var parentExists = await _repository.ExistsAsync(request.ParentId.Value, cancellationToken);
      if (!parentExists) {
        throw new InvalidOperationException($"Parent token with id {request.ParentId} not found");
      }
    }

    string candidateToken = await _repository.FindUnusedNewToken(cancellationToken);
    // Create new token
    var token = new AccessToken(
        candidateToken,
        request?.IssuedTo ?? "unspecified",
        DateTime.UtcNow.AddMinutes(request?.ExpiresInMinutes ?? 60),
        request?.ParentId);

    await _repository.AddAsync(token, cancellationToken);
    await _repository.SaveChangesAsync(cancellationToken);

    // Return DTO
    return new AccessTokenDto {
      Id = token.Id,
      ParentId = token.ParentId,
      Token = token.Token,      
      Created = token.Created,
      Expires = token.Expires,
      Used = token.Used,
      IsValid = token.IsValid("newUnused"),
      IsExpired = token.IsExpired()
    };
  }
}
