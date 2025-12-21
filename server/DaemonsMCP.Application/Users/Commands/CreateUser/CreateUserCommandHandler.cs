using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using MediatR;

namespace DaemonsMCP.Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto> {
  private readonly IUserRepository _repository;

  public CreateUserCommandHandler(IUserRepository repository) {
    _repository = repository;
  }

  public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken) {
    // Check if email already exists
    if (await _repository.EmailExistsAsync(request.Email, cancellationToken)) {
      throw new InvalidOperationException($"User with email {request.Email} already exists");
    }

    // Create user entity
    User user;
    if (!string.IsNullOrEmpty(request.Password)) {
      // TODO: Hash password properly (use BCrypt or similar)
      var passwordHash = HashPassword(request.Password);
      user = User.CreateWithPassword(request.Email, passwordHash, request.DisplayName);
    } else {
      user = User.CreateWithOAuth(request.Email, request.DisplayName, request.GoogleId, request.GitHubId);
    }

    await _repository.AddAsync(user, cancellationToken);
    await _repository.SaveChangesAsync(cancellationToken);

    return MapToDto(user);
  }

  private static string HashPassword(string password) {
    // TODO: Implement proper password hashing (BCrypt.Net-Next recommended)
    return BCrypt.Net.BCrypt.HashPassword(password);
  }

  private static UserDto MapToDto(User user) {
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
