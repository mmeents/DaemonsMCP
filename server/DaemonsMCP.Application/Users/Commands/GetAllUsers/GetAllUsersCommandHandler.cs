using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using MediatR;

namespace DaemonsMCP.Application.Users.Commands.GetAllUsers {

  public class GetAllUsersCommand : IRequest<List<UserDto>> {
  }

  internal class GetAllUsersCommandHandler : IRequestHandler<GetAllUsersCommand, List<UserDto>> {
    private readonly IUserRepository _userRepository;

    public GetAllUsersCommandHandler(IUserRepository userRepository) {
      _userRepository = userRepository;
    }

    public async Task<List<UserDto>> Handle(GetAllUsersCommand request, CancellationToken cancellationToken) {
      var users = await _userRepository.GetAllAsync(cancellationToken);
      return users.Select(u => u.ToDto()).ToList();
    }
  }
}
