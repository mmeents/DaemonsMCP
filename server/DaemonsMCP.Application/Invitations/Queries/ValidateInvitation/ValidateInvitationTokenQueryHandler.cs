using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;

namespace DaemonsMCP.Application.Invitations.Queries.ValidateInvitation {

  public record ValidateInvitationTokenQuery(string Token) : IRequest<InvitationTokenDto?>;

  public class ValidateInvitationTokenQueryHandler : IRequestHandler<ValidateInvitationTokenQuery, InvitationTokenDto?> {
    private readonly IInvitationTokenRepository _repository;

    public ValidateInvitationTokenQueryHandler(IInvitationTokenRepository repository) {
      _repository = repository;
    }

    public async Task<InvitationTokenDto?> Handle(ValidateInvitationTokenQuery request, CancellationToken cancellationToken) {

      var token = await _repository.GetByTokenAsync(request.Token, cancellationToken);
      if (token == null || !token.IsValid()) {
        return null;
      }
      return token.ToDto();

    }
  }
}
