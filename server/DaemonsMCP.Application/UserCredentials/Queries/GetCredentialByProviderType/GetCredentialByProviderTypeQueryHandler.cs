using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Enums;
using DaemonsMCP.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Application.UserCredentials.Queries.GetCredentialByProviderType {
  public class GetCredentialByProviderTypeQuery : IRequest<UserCredentialDto?> {
    public int UserId { get; set; }
    public ProviderType ProviderType { get; set; }
  }

  public class GetCredentialByProviderTypeQueryHandler : IRequestHandler<GetCredentialByProviderTypeQuery, UserCredentialDto?> {
    private readonly IUserCredentialRepository _userCredentialRepository;

    public GetCredentialByProviderTypeQueryHandler(IUserCredentialRepository userCredentialRepository) {
      _userCredentialRepository = userCredentialRepository;
    }

    public async Task<UserCredentialDto?> Handle(GetCredentialByProviderTypeQuery request, CancellationToken cancellationToken) {
      var credential = await _userCredentialRepository.GetByUserAndProviderAsync(request.UserId, request.ProviderType);
      return credential?.ToDto();
    }
  }
}
