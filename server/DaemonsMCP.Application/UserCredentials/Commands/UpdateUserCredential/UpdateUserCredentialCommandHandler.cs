using MediatR;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using System;
using LibGit2Sharp;


namespace DaemonsMCP.Application.UserCredentials.Commands.UpdateUserCredential {

  public class UpdateUserCredentialCommand : IRequest<UserCredentialDto> {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Username { get; set; } // Null = don't update
    public string? Secret { get; set; }   // Null = don't update
    public bool? IsActive { get; set; }   // Null = don't update    
  }

  internal class UpdateUserCredentialCommandHandler : IRequestHandler<UpdateUserCredentialCommand, UserCredentialDto> {
    private readonly IUserCredentialRepository _userCredentialRepository;
    private readonly ICredentialEncryptionService _protector;

    public UpdateUserCredentialCommandHandler(IUserCredentialRepository userCredentialRepository, ICredentialEncryptionService protector) {
      _userCredentialRepository = userCredentialRepository;
      _protector = protector;
    }

    public async Task<UserCredentialDto> Handle(UpdateUserCredentialCommand request, CancellationToken cancellationToken) {
      var credential = await _userCredentialRepository.GetByIdAsync(request.Id);
      if (credential == null) throw new NotFoundException("UserCredential not found");

      credential.Name = request.Name;
      if (request.Username != null) {
        credential.EncryptedUsername = _protector.Encrypt(request.Username);
      }
      if (request.Secret != null) {
        credential.EncryptedSecret = _protector.Encrypt(request.Secret) ;
      }
      if (request.IsActive != null) {
        credential.IsActive = request.IsActive ?? credential.IsActive;
      } 

      await _userCredentialRepository.UpdateAsync(credential);
      return credential.ToDto();
    }
  }
}
