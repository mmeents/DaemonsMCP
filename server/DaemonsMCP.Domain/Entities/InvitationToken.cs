using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Entities {
  public class InvitationToken {
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public string? InvitedEmail { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? UsedAt { get; set; }
    public int? UsedByUserId { get; set; }

    // Navigation properties
    public User CreatedBy { get; set; } = null!;
    public User? UsedBy { get; set; }

    private InvitationToken() { }

    public static InvitationToken Create(string token, int createdByUserId, int expiresInHours = 168, string? invitedEmail = null) {
      return new InvitationToken {
        Token = token,
        InvitedEmail = invitedEmail?.ToLowerInvariant(),
        CreatedByUserId = createdByUserId,
        CreatedAt = DateTime.UtcNow,
        ExpiresAt = DateTime.UtcNow.AddHours(expiresInHours),
        IsUsed = false
      };
    }

    public bool IsValid() {
      return !IsUsed && DateTime.UtcNow < ExpiresAt;
    }

    public void MarkAsUsed(int usedByUserId) {
      if (IsUsed) {
        throw new InvalidOperationException("Invitation has already been used");
      }
      if (DateTime.UtcNow >= ExpiresAt) {
        throw new InvalidOperationException("Invitation has expired");
      }

      IsUsed = true;
      UsedAt = DateTime.UtcNow;
      UsedByUserId = usedByUserId;
    }
  }
}
