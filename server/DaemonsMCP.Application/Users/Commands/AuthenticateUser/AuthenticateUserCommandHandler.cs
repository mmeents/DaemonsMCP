using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using MediatR;

namespace DaemonsMCP.Application.Users.Commands.AuthenticateUser;

public class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, UserDto?> {
  private readonly IUserRepository _repository;

  public AuthenticateUserCommandHandler(IUserRepository repository) {
    _repository = repository;
  }

  public async Task<UserDto?> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken) {
    var user = await _repository.GetByEmailAsync(request.Email, cancellationToken);
    if (user == null || string.IsNullOrEmpty(user.PasswordHash)) {
      return null;
    }

    // Verify password
    if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) {
      return null;
    }

    // Update last login
    user.RecordLogin();
    await _repository.UpdateAsync(user, cancellationToken);
    await _repository.SaveChangesAsync(cancellationToken);

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
