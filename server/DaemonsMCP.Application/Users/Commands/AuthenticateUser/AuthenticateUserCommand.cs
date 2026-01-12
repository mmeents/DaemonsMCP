using DaemonsMCP.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Application.Users.Commands.AuthenticateUser {
  public record AuthenticateUserCommand(
    string Email,
    string Password
  ) : IRequest<UserDto?>;

}
