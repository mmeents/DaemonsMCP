using DaemonsMCP.Domain.Enums;
using DaemonsMCP.Domain.Entities;

namespace DaemonsMCP.Domain.Models {

  public class UserCredentialDto {
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ProviderType ProviderType { get; set; }
    public CredentialType CredentialType { get; set; }

    // Security: Never expose encrypted values in DTOs sent to client
    public bool HasUsername { get; set; }  // Just indicate if set
    public bool HasSecret { get; set; }     // Just indicate if set

    public DateTime CreatedDate { get; set; }
    public DateTime? LastUsedDate { get; set; }
    public bool IsActive { get; set; }
  }

  public static class UserCredentialDtoExtensions {
    public static UserCredentialDto ToDto(this UserCredential credential) {
      return new UserCredentialDto {
        Id = credential.Id,
        UserId = credential.UserId,
        Name = credential.Name,
        ProviderType = credential.ProviderType,
        CredentialType = credential.CredentialType,
        HasUsername = !string.IsNullOrEmpty(credential.EncryptedUsername),
        HasSecret = !string.IsNullOrEmpty(credential.EncryptedSecret),
        CreatedDate = credential.CreatedDate,
        LastUsedDate = credential.LastUsedDate,
        IsActive = credential.IsActive
      };
    }
  }
}
