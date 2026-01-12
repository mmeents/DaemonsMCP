using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using MediatR;


namespace DaemonsMCP.Application.Invitations.Queries.GetInvitationTokenById {

  public record GetInvitationTokenByIdQuery(int Id) : IRequest<InvitationTokenDto?>;

  public class GetInvitationTokenByIdQueryHandler : IRequestHandler<GetInvitationTokenByIdQuery, InvitationTokenDto?> {
    private readonly IInvitationTokenRepository _repository;

    public GetInvitationTokenByIdQueryHandler(IInvitationTokenRepository repository) {
      _repository = repository;
    }

    public async Task<InvitationTokenDto?> Handle(GetInvitationTokenByIdQuery request, CancellationToken cancellationToken) {
      var token = await _repository.GetByIdAsync(request.Id);
      return token is not null ? token.ToDto() : null;
    }
  }
}
