using DaemonsMCP.Domain.Models;
using MediatR;


namespace DaemonsMCP.Application.Users.Commands.GetUserById {
  public record GetUserByIdQuery(int Id) : IRequest<UserDto?>;
}
