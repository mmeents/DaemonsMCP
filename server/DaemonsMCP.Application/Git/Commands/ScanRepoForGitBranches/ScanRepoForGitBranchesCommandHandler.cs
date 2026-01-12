using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using LibGit2Sharp;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Application.Git.Commands.ScanRepoForGitBranches {

  public record ScanRepoForGitBranchesCommand(int getRepositoryId) : IRequest<bool>;
  internal class ScanRepoForGitBranchesCommandHandler : IRequestHandler<ScanRepoForGitBranchesCommand, bool> {
    private readonly IGitRepositoryRepository _gitRepositoryRepository;
    private readonly IGitBranchRepository _gitBranchRepository;

    public ScanRepoForGitBranchesCommandHandler(
      IGitRepositoryRepository gitRepositoryRepository,
      IGitBranchRepository gitBranchRepository) {
      _gitRepositoryRepository = gitRepositoryRepository;
      _gitBranchRepository = gitBranchRepository;
    }

    public async Task<bool> Handle(ScanRepoForGitBranchesCommand request, CancellationToken cancellationToken) {
      List<GitBranchDto> branches = new List<GitBranchDto>();
      int RepositoryId = request.getRepositoryId;

      // 1. Get GitRepository from DB
      var DbBranches = await _gitRepositoryRepository.GetByIdAsync(RepositoryId);
      if (DbBranches == null) {
        return false;
      }

      var projectId = DbBranches.ProjectId;
      var repoPath = Path.GetFullPath(Path.Combine(DbBranches.Project.RootPath, DbBranches.LocalPath));
      var repoParentPath = Directory.GetParent(repoPath)?.FullName;

      // 2. Open repo with LibGit2Sharp
      using var repo = new LibGit2Sharp.Repository(repoParentPath);

      // 3. List all branches (local + remote)
      foreach (var branch in repo.Branches) {
        var isRemote = branch.IsRemote;
        var branchName = isRemote ? branch.FriendlyName.Replace("origin/", "") : branch.FriendlyName;
        var trackingBranchName = branch.TrackedBranch?.FriendlyName;

        var lastCommit = branch.Tip;
        var lastCommitSha = lastCommit?.Sha;
        var lastCommitMessage = lastCommit?.MessageShort;
        var lastCommitAuthor = lastCommit?.Author.Name;
        var lastCommitDate = lastCommit?.Author.When.DateTime;

        branches.Add(new GitBranchDto {
          GitRepositoryId = RepositoryId,
          BranchName = branchName,
          FullName = branch.FriendlyName,
          IsRemote = isRemote,
          TrackingBranchName = trackingBranchName,
          LastCommitSha = lastCommitSha,
          LastCommitMessage = lastCommitMessage,
          LastCommitAuthor = lastCommitAuthor,
          LastCommitDate = lastCommitDate,
          CreatedAt = DateTime.UtcNow,
          UpdatedAt = DateTime.UtcNow
        });
      }

      // 4. Synchronize with DB GitBranch entries
      var existingBranches = DbBranches.Branches.ToDictionary(b => b.FullName, b => b);
      foreach (var branchDto in branches) {
        if (existingBranches.TryGetValue(branchDto.FullName, out var existingBranch)) {
          // Update existing branch
          existingBranch.BranchName = branchDto.BranchName;
          existingBranch.IsRemote = branchDto.IsRemote;
          existingBranch.TrackingBranchName = branchDto.TrackingBranchName;
          existingBranch.LastCommitSha = branchDto.LastCommitSha;
          existingBranch.LastCommitMessage = branchDto.LastCommitMessage;
          existingBranch.LastCommitAuthor = branchDto.LastCommitAuthor;
          existingBranch.LastCommitDate = branchDto.LastCommitDate;
          existingBranch.UpdatedAt = DateTime.UtcNow;

          await _gitBranchRepository.UpdateAsync(existingBranch, cancellationToken);
        } else {
          // Add new branch
          var newBranch = new DaemonsMCP.Domain.Entities.GitBranch {
            GitRepositoryId = RepositoryId,
            BranchName = branchDto.BranchName,
            FullName = branchDto.FullName,
            IsRemote = branchDto.IsRemote,
            TrackingBranchName = branchDto.TrackingBranchName,
            LastCommitSha = branchDto.LastCommitSha,
            LastCommitMessage = branchDto.LastCommitMessage,
            LastCommitAuthor = branchDto.LastCommitAuthor,
            LastCommitDate = branchDto.LastCommitDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
          };
          await _gitBranchRepository.AddAsync(newBranch, cancellationToken);
        }
      }
      await _gitBranchRepository.SaveChangesAsync(cancellationToken);


      return true;
    }
  }
}
