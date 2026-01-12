using DaemonsMCP.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Application.Invitations.Commands.RevokeInvitation {
  public record RevokeInvitationCommand(int InvitationId) : IRequest<bool>;

  public class RevokeInvitationCommandHandler : IRequestHandler<RevokeInvitationCommand, bool> {
    private IInvitationTokenRepository _repository;
    public RevokeInvitationCommandHandler(IInvitationTokenRepository repository) {
      _repository = repository;
    }


    public async Task<bool> Handle(
      RevokeInvitationCommand request, 
      CancellationToken cancellationToken) 
    {
      var invite = await _repository.GetByIdAsync(request.InvitationId, cancellationToken);

      if (invite == null) {
        throw new Exception($"Invitation with ID {request.InvitationId} not found");
      }

      if (invite.ExpiresAt > DateTime.UtcNow) {
        invite.ExpiresAt = DateTime.UtcNow;
        await _repository.UpdateAsync(invite, cancellationToken);
      }     

      // Logic to revoke the invitation
      return true;
    }
  }
}
