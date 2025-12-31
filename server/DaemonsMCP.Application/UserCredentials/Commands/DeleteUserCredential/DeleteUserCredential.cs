using MediatR;
using DaemonsMCP.Domain.Repositories; 
using DaemonsMCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Application.UserCredentials.Commands.DeleteUserCredential {

  public record DeleteUserCredentialCommand(int Id) : IRequest<bool>;
  public class DeleteUserCredentialCommandHandler : IRequestHandler<DeleteUserCredentialCommand, bool> {
    private readonly IUserCredentialRepository _userCredentialRepository;

    public DeleteUserCredentialCommandHandler(IUserCredentialRepository userCredentialRepository) {
      _userCredentialRepository = userCredentialRepository;
    }

    public async Task<bool> Handle(DeleteUserCredentialCommand request, CancellationToken cancellationToken) {
      var credential = await _userCredentialRepository.GetByIdAsync(request.Id);
      if (credential == null) return false;

      await _userCredentialRepository.DeleteAsync(credential.Id);
      return true;
    }
  }
}
