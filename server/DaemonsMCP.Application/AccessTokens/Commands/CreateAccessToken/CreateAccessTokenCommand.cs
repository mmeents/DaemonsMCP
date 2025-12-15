using DaemonsMCP.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Application.AccessTokens.Commands.CreateAccessToken {
  public record CreateAccessTokenCommand(
    string? Token,   
    string? IssuedTo = "unknown",
    int? ParentId = null,
    int ExpiresInMinutes = 60
  ) : IRequest<AccessTokenDto>;
}
