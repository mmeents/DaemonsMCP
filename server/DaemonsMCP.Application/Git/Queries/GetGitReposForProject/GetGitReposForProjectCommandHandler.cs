using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;
using MediatR;

namespace DaemonsMCP.Application.Git.Queries.GetGitReposForProject {

  public record GetGitReposForProjectCommand(int ProjectId) : IRequest<List<GitRepositoryDto>>;
  internal class GetGitReposForProjectCommandHandler : IRequestHandler<GetGitReposForProjectCommand, List<GitRepositoryDto>> {
    private readonly IGitRepositoryRepository _gitRepositoryRepository;

    public GetGitReposForProjectCommandHandler(IGitRepositoryRepository gitRepositoryRepository) {
      _gitRepositoryRepository = gitRepositoryRepository;
    }

    public async Task<List<GitRepositoryDto>> Handle(GetGitReposForProjectCommand request, CancellationToken cancellationToken) {
      var gitRepos = await _gitRepositoryRepository.GetByProjectIdAsync(request.ProjectId);
      return gitRepos.Select(repo => repo.ToDto()).ToList();
    }
  }
}
