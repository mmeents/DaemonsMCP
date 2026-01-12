using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Entities {
  public class GitBranch {
    public int Id { get; set; }
    public int GitRepositoryId { get; set; }

    public string BranchName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public bool IsRemote { get; set; }
    public bool IsCurrentBranch { get; set; }
    public bool IsTracking { get; set; }
    public string? TrackingBranchName { get; set; }

    public string? LastCommitSha { get; set; }
    public string? LastCommitMessage { get; set; }
    public string? LastCommitAuthor { get; set; }
    public DateTime? LastCommitDate { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public GitRepository GitRepository { get; set; } = null!;
  }
}
