using DaemonsMCP.Domain.Models;
using MediatR;

namespace DaemonsMCP.Application.Users.Commands.CreateUser;

public record CreateUserCommand(
  string Email,
  string DisplayName,
  string? Password = null,
  string? GoogleId = null,
  string? GitHubId = null
) : IRequest<UserDto>;
