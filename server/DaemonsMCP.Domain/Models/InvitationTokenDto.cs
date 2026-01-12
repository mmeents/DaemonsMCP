using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Models {
  public class InvitationTokenDto {
    public int Id { get; init; }
    public string Token { get; init; } = string.Empty;
    public string? InvitedEmail { get; init; }
    public int CreatedByUserId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime ExpiresAt { get; init; }
    public bool IsUsed { get; set; }
    public DateTime? UsedAt { get; set; }
    public int? UsedByUserId { get; set; }



  }


  public static class InvitationTokenDtoExtensions {
    public static InvitationTokenDto ToDto(this Entities.InvitationToken entity) {
      return new InvitationTokenDto {
        Id = entity.Id,
        Token = entity.Token,
        InvitedEmail = entity.InvitedEmail,
        CreatedByUserId = entity.CreatedByUserId,
        CreatedAt = entity.CreatedAt,
        ExpiresAt = entity.ExpiresAt,
        IsUsed = entity.IsUsed,
        UsedAt = entity.UsedAt,
        UsedByUserId = entity.UsedByUserId
      };
    }
  }


}
