using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Models {
  public class GitRepositoryDto {
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
    public string? LastSyncStatus { get; set; }
    public string? LastSyncError { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
  }

  public static class GitRepositoryDtoExtensions {
    public static GitRepositoryDto ToDto(this Entities.GitRepository gitRepository) {
      return new GitRepositoryDto {
        Id = gitRepository.Id,
        ProjectId = gitRepository.ProjectId,
        UserCredentialId = gitRepository.UserCredentialId,
        RemoteUrl = gitRepository.RemoteUrl,
        LocalPath = gitRepository.LocalPath,
        RemoteName = gitRepository.RemoteName,
        CurrentBranchName = gitRepository.CurrentBranchName,
        IsDirty = gitRepository.IsDirty,
        ModifiedFileCount = gitRepository.ModifiedFileCount,
        UntrackedFileCount = gitRepository.UntrackedFileCount,
        LastFetchedAt = gitRepository.LastFetchedAt,
        LastSyncedAt = gitRepository.LastSyncedAt,
        LastSyncStatus = gitRepository.LastSyncStatus.ToString(),
        LastSyncError = gitRepository.LastSyncError,
        CreatedAt = gitRepository.CreatedAt,
        UpdatedAt = gitRepository.UpdatedAt,
        IsDeleted = gitRepository.IsDeleted
      };
    }
  }
}
