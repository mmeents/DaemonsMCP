using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Models {
  public class GitBranchDto {
    public int Id { get; set; }
    public int GitRepositoryId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public bool IsRemote { get; set; }
    public string? TrackingBranchName { get; set; } 
    public string? LastCommitSha { get; set; }
    public string? LastCommitMessage { get; set; }
    public string? LastCommitAuthor { get; set; }
    public DateTime? LastCommitDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

  }

  public static class GitBranchDtoExtensions {
    public static GitBranchDto ToDto(this Entities.GitBranch gitBranch) {
      return new GitBranchDto {
        Id = gitBranch.Id,
        GitRepositoryId = gitBranch.GitRepositoryId,
        BranchName = gitBranch.BranchName,
        FullName = gitBranch.FullName,
        IsRemote = gitBranch.IsRemote,
        TrackingBranchName = gitBranch.TrackingBranchName,
        LastCommitSha = gitBranch.LastCommitSha,
        LastCommitMessage = gitBranch.LastCommitMessage,
        LastCommitAuthor = gitBranch.LastCommitAuthor,
        LastCommitDate = gitBranch.LastCommitDate,
        CreatedAt = gitBranch.CreatedAt,
        UpdatedAt = gitBranch.UpdatedAt
      };
    }
  }
}
