using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaemonsMCP.Domain.Models;
using DaemonsMCP.Domain.Repositories;

namespace DaemonsMCP.Application.Git.Queries.GetGitBranchesByRepo {

  public record GetGitBranchesByRepoQuery(int GitRepositoryId) : IRequest<List<GitBranchDto>>;

  public class GetGitBranchesByRepoQueryHandler : IRequestHandler<GetGitBranchesByRepoQuery, List<GitBranchDto>> {
    private readonly IGitRepositoryRepository _gitRepositoryRepository;    
    public GetGitBranchesByRepoQueryHandler(      
      IGitRepositoryRepository gitRepositoryRepository 
    ) {
      _gitRepositoryRepository = gitRepositoryRepository;      
    }
    public async Task<List<GitBranchDto>> Handle(GetGitBranchesByRepoQuery request, CancellationToken cancellationToken) {
      List<GitBranchDto> branches = new List<GitBranchDto>();
      int RepositoryId = request.GitRepositoryId;

      // 1. Get GitRepository from DB
      var DbBranches = await _gitRepositoryRepository.GetByIdAsync(RepositoryId);
      if (DbBranches == null) {
        return branches;
      }
      
      // 3. List all branches (local + remote)
      foreach (var branch in DbBranches.Branches) {    
        branches.Add(branch.ToDto());
      }   
      
      return branches;
    }
  }

  

  
}
