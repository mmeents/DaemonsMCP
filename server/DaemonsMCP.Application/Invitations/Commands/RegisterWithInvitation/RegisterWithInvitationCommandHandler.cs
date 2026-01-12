using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;

namespace DaemonsMCP.Application.Invitations.Commands.RegisterWithInvitation {


  public record RegisterWithInvitationCommand(
    string InvitationToken,
    string Email,
    string DisplayName,
    string Password
  ) : IRequest<UserDto?>;

  public class RegisterWithInvitationCommandHandler(
    IInvitationTokenRepository repository, 
    IUserRepository userRepository) : IRequestHandler<RegisterWithInvitationCommand, UserDto?> {
    private readonly IInvitationTokenRepository _repository = repository;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<UserDto?> Handle(RegisterWithInvitationCommand request, CancellationToken cancellationToken) {

      var token = await _repository.GetByTokenAsync(request.InvitationToken, cancellationToken);
      if (token == null || !token.IsValid()) {
        return null;
      }

      if (!string.IsNullOrEmpty(token.InvitedEmail) && 
        !token.InvitedEmail.Equals(request.Email, StringComparison.OrdinalIgnoreCase)) {
        throw new InvalidOperationException("Invitation is for a different email address");
      }

      // Check if user already exists
      var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
      if (existingUser != null) {
        throw new InvalidOperationException("User with this email already exists");
      }

      var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

      using var transaction = await _repository.BeginTransactionAsync(cancellationToken);
      try {
        var user = User.CreateWithPassword(request.Email, passwordHash, request.DisplayName);
        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null) { 
          throw new Exception("User creation failed");
        }

        token.MarkAsUsed( user.Id);
        await _repository.UpdateAsync(token, cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        return user.ToDto();

      } catch (Exception) { 
        await transaction.RollbackAsync(cancellationToken);
        throw;
      }

    }
  }
}
