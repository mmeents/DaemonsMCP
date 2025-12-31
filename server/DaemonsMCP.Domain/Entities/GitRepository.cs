using DaemonsMCP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Entities {
  public class GitRepository {
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int? UserCredentialId { get; set; }
    public string RemoteUrl { get; set; } = string.Empty;
    public string LocalPath { get; set; } = string.Empty;
    public string RemoteName { get; set; } = "origin";

    public string? CurrentBranchName { get; set; }
    public bool IsDirty { get; set; } = false;
    public int? ModifiedFileCount { get; set; }
    public int? UntrackedFileCount { get; set; }

    public DateTime? LastFetchedAt { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public SyncStatus LastSyncStatus { get; set; } = SyncStatus.Unknown;
    public string? LastSyncError { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; 
    public bool IsDeleted { get; set; } = false;

    // Navigation properties
    public Project Project { get; set; } = null!;
    public UserCredential? UserCredential { get; set; }
    public ICollection<GitBranch> Branches { get; set; } = new List<GitBranch>(); 
  }
}
