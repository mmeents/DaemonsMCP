namespace DaemonsMCP.Domain.Entities;

public class AccessToken {
  public int Id { get; private set; }
  public int? ParentId { get; private set; }  
  public string Token { get; private set; } = string.Empty;
  public string IssuedTo { get; set; } = "unknown";
  public DateTime Created { get; private set; }
  public DateTime Expires { get; private set; }
  public DateTime? Used { get; private set; }
  public string? UsedUrl { get; private set; }
  public string? UsedUrlNextToken { get; set; }
  public string? UsedBy { get; private set; }
  public bool IsRevokedChain { get; set; } = false;

  // Navigation properties
  public AccessToken? Parent { get; private set; }
  public ICollection<AccessToken> Children { get; private set; } = new List<AccessToken>();

  // EF Core constructor
  private AccessToken() {
    Created = DateTime.UtcNow;
  }

  public AccessToken(string token, string issuedTo, DateTime expires, int? parentId = null) {
    Token = token;
    IssuedTo = issuedTo;
    Expires = expires;
    ParentId = parentId;
    Created = DateTime.UtcNow;
  }

  public void MarkAsUsed(string usedUrl, string? usedBy, string? usedUrlNextToken)
  {
    Used = DateTime.UtcNow;
    Expires = DateTime.UtcNow.AddMinutes(3); // AI use parallelism and may make same request multiple times within short time
    UsedUrl = usedUrl;
    UsedBy = usedBy;
    UsedUrlNextToken = usedUrlNextToken;
  }

  public bool IsExpired() => DateTime.UtcNow > Expires;

  public bool IsValid(string usedUrl) => 
    !IsExpired() 
    && ( UsedUrl == usedUrl || UsedUrl == null)  // allow re-use for same URL within validity period
    && !(Parent?.IsRevokedChain ?? false);
}
