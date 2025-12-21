using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Domain.Entities;

namespace DaemonsMCP.Application.Invitations.Commands.CreateInvitationToken {

  public record CreateInvitationCommand(
    int CreatedByUserId,
    string? InvitedEmail = null,
    int ExpiresInHours = 168 // Default 7 days
  ) : IRequest<InvitationTokenDto>;

  public class CreateInvitationCommandHandler : IRequestHandler<CreateInvitationCommand, InvitationTokenDto> {
    private readonly IInvitationTokenRepository _repository;

    public CreateInvitationCommandHandler(IInvitationTokenRepository repository) {
      _repository = repository;
    }

    public async Task<InvitationTokenDto> Handle(CreateInvitationCommand request, CancellationToken cancellationToken) {

      
      string aNewToken = await _repository.FindUnusedNewToken(cancellationToken);

      var newToken = InvitationToken.Create(
        token: aNewToken,
        createdByUserId: request.CreatedByUserId,
        expiresInHours: request.ExpiresInHours,
        invitedEmail: request.InvitedEmail
      );    

      var returnToken = await _repository.AddAsync(newToken, cancellationToken);     
      if (returnToken == null) { 
        throw new InvalidOperationException("Failed to create invite.");
      }

      return returnToken.ToDto();
    }
  }
}
