using MediatR;
using DaemonsMCP.Domain.Enums;
using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;

namespace DaemonsMCP.Application.UserCredentials.Commands.CreateUserCredential {

  public class CreateUserCredentialCommand : IRequest<UserCredentialDto> {
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ProviderType ProviderType { get; set; }
    public CredentialType CredentialType { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty; // PersonalAccessToken, password, etc.
  }

  public class CreateUserCredentialCommandHandler : IRequestHandler<CreateUserCredentialCommand, UserCredentialDto> {
    private readonly IUserCredentialRepository _userCredentialRepository;
    private readonly ICredentialEncryptionService _protector;

    public CreateUserCredentialCommandHandler(
      IUserCredentialRepository userCredentialRepository,
      ICredentialEncryptionService protector) {
      _userCredentialRepository = userCredentialRepository;
      _protector = protector;
    }

    public async Task<UserCredentialDto> Handle(CreateUserCredentialCommand request, CancellationToken cancellationToken) {
      var userCredential = new UserCredential {
        UserId = request.UserId,
        Name = request.Name,
        ProviderType = request.ProviderType,
        CredentialType = request.CredentialType,
        EncryptedUsername = _protector.Encrypt(request.Username),
        EncryptedSecret = _protector.Encrypt(request.Secret)
      };

      await _userCredentialRepository.AddAsync(userCredential);
      return userCredential.ToDto();
    }
  }
}
