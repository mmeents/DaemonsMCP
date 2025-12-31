using MediatR;
using DaemonsMCP.Domain.Repositories;
using DaemonsMCP.Domain.Models;
using Microsoft.Extensions.Logging;
using LibGit2Sharp;



namespace DaemonsMCP.Application.Git.Commands.ScanProjectForGitRepos {

  public record ScanProjectForGitReposCommand(int ProjectId) : IRequest<List<GitRepositoryDto>>;

  internal class ScanProjectForGitReposCommandHandler : IRequestHandler<ScanProjectForGitReposCommand, List<GitRepositoryDto>> {
    private readonly IGitRepositoryRepository _gitRepositoryRepository;
    private readonly IProjectRepository _projectRepository;
    private ILogger<ScanProjectForGitReposCommandHandler> _logger;

    public ScanProjectForGitReposCommandHandler(IGitRepositoryRepository gitRepositoryRepository, IProjectRepository projectRepository, ILogger<ScanProjectForGitReposCommandHandler> logger) {
      _gitRepositoryRepository = gitRepositoryRepository;
      _projectRepository = projectRepository;
      _logger = logger;
    }

    public async Task<List<GitRepositoryDto>> Handle(ScanProjectForGitReposCommand request, CancellationToken cancellationToken) {
      var discoveredRepos = new List<GitRepositoryDto>();

      // 1. Get project root path
      var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
      if (project == null) {
        _logger.LogWarning("Project with ID {ProjectId} not found", request.ProjectId);
        return discoveredRepos;
      }

      _logger.LogInformation("Scanning project {ProjectId} at path {RootPath} for Git repositories",
          request.ProjectId, project.RootPath);

      // 2. Recursively search for .git folders
      var gitDirectories = Directory.EnumerateDirectories(project.RootPath, ".git", SearchOption.AllDirectories);

      // 3. Process each .git folder found
      foreach (var gitDirectory in gitDirectories) {
        try {
          // Get repository root (parent of .git folder)
          var repoPath = Directory.GetParent(gitDirectory)?.FullName;
          if (string.IsNullOrEmpty(repoPath)) {
            _logger.LogWarning("Could not determine repository path for {GitDirectory}", gitDirectory);
            continue;
          }

          // Calculate relative path from project root
          var relativePath = Path.GetRelativePath(project.RootPath, gitDirectory);

          // Security: Validate path doesn't contain traversal attempts
          if (relativePath.Contains("..") || relativePath.Contains("~")) {
            _logger.LogWarning("Rejected unsafe path: {Path}", relativePath);
            continue;
          }

          // Normalize path separators for cross-platform compatibility
          relativePath = relativePath.Replace("\\", "/");

          // Open repository with LibGit2Sharp
          using var repo = new LibGit2Sharp.Repository(repoPath);

          // Extract repository information
          var currentBranch = repo.Head.FriendlyName;
          var remote = repo.Network.Remotes.FirstOrDefault();
          var remoteUrl = remote?.Url ?? string.Empty;
          var remoteName = remote?.Name ?? "origin";

          // Get repository status
          var status = repo.RetrieveStatus();
          var isDirty = status.IsDirty;
          var modifiedCount = status.Modified.Count();
          var untrackedCount = status.Untracked.Count();

          _logger.LogDebug("Found Git repository at {RepoPath}: Branch={Branch}, Remote={RemoteUrl}, Dirty={IsDirty}",
              relativePath, currentBranch, remoteUrl, isDirty);

          // Check if repo already exists in DB
          var existingRepo = await _gitRepositoryRepository.GetByPathAsync(request.ProjectId, relativePath, cancellationToken);

          if (existingRepo == null) {
            // Insert new GitRepository record
            var newRepo = new Domain.Entities.GitRepository {
              ProjectId = request.ProjectId,
              LocalPath = relativePath,
              RemoteUrl = remoteUrl,
              RemoteName = remoteName,
              CurrentBranchName = currentBranch,
              IsDirty = isDirty,
              ModifiedFileCount = modifiedCount,
              UntrackedFileCount = untrackedCount,
              CreatedAt = DateTime.UtcNow,
              UpdatedAt = DateTime.UtcNow
            };

            await _gitRepositoryRepository.AddAsync(newRepo, cancellationToken);
            discoveredRepos.Add(newRepo.ToDto());

            _logger.LogInformation("Discovered new Git repository: {LocalPath}", relativePath);
          } else {
            // Update existing GitRepository record
            existingRepo.RemoteUrl = remoteUrl;
            existingRepo.RemoteName = remoteName;
            existingRepo.CurrentBranchName = currentBranch;
            existingRepo.IsDirty = isDirty;
            existingRepo.ModifiedFileCount = modifiedCount;
            existingRepo.UntrackedFileCount = untrackedCount;
            existingRepo.UpdatedAt = DateTime.UtcNow;

            await _gitRepositoryRepository.UpdateAsync(existingRepo, cancellationToken);
            discoveredRepos.Add(existingRepo.ToDto());

            _logger.LogInformation("Updated existing Git repository: {LocalPath}", relativePath);
          }

        } catch (Exception ex) {
          _logger.LogError(ex, "Error processing Git repository at path {GitDirectory}", gitDirectory);
          // Continue processing other repositories
        }
      }

      // Save all changes once at the end
      await _gitRepositoryRepository.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Scan complete: Found {Count} Git repositories in project {ProjectId}",
          discoveredRepos.Count, request.ProjectId);

      return discoveredRepos;
    }
  }
}
