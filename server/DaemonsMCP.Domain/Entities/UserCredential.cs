using DaemonsMCP.Domain.Enums;

namespace DaemonsMCP.Domain.Entities {
  public class UserCredential {
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ProviderType ProviderType { get; set; }
    public CredentialType CredentialType { get; set; }
    public string? EncryptedUsername { get; set; }
    public string EncryptedSecret { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? LastUsedDate { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<GitRepository> GitRepositories { get; set; } = new List<GitRepository>();
  }
}
