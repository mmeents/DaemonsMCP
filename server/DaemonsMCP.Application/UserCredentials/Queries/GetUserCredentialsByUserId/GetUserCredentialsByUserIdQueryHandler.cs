using DaemonsMCP.Domain.Models;
using MediatR;
using DaemonsMCP.Domain.Enums;
using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Repositories;

namespace DaemonsMCP.Application.UserCredentials.Queries.GetUserCredentialsByUserId {

  public class GetUserCredentialsByUserIdQuery : IRequest<List<UserCredentialDto>> {
    public int UserId { get; set; }
  }

  public class GetUserCredentialsByUserIdQueryHandler : IRequestHandler<GetUserCredentialsByUserIdQuery, List<UserCredentialDto>> {
    private readonly IUserCredentialRepository _userCredentialRepository;

    public GetUserCredentialsByUserIdQueryHandler(IUserCredentialRepository userCredentialRepository) {
      _userCredentialRepository = userCredentialRepository;
    }

    public async Task<List<UserCredentialDto>> Handle(GetUserCredentialsByUserIdQuery request, CancellationToken cancellationToken) {
      var credentials = await _userCredentialRepository.GetByUserIdAsync(request.UserId);
      return credentials.Select(c => c.ToDto()).ToList();
    }
  }
}
