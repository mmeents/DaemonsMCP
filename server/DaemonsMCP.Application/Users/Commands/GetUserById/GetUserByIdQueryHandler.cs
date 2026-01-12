using DaemonsMCP.Application.Users.Commands.GetUserById;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using MediatR;

namespace DaemonsMCP.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?> {
  private readonly IUserRepository _repository;

  public GetUserByIdQueryHandler(IUserRepository repository) {
    _repository = repository;
  }

  public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken) {
    var user = await _repository.GetByIdAsync(request.Id, cancellationToken);
    if (user == null) return null;

    return new UserDto {
      Id = user.Id,
      Email = user.Email,
      DisplayName = user.DisplayName,
      HasPassword = !string.IsNullOrEmpty(user.PasswordHash),
      HasGoogleAuth = !string.IsNullOrEmpty(user.GoogleId),
      HasGitHubAuth = !string.IsNullOrEmpty(user.GitHubId),
      CreatedAt = user.CreatedAt,
      LastLoginAt = user.LastLoginAt,
      IsActive = user.IsActive
    };
  }
}
