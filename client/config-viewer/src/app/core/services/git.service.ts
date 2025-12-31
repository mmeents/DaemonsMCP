import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { GitRepositoryDto, GitBranchDto } from '../../shared/models/api.models';

@Injectable({
  providedIn: 'root'
})
export class GitService extends ApiService {
  
  /**
   * Get all Git repositories for a project from database
   */
  getGitReposForProject(projectId: number): Observable<GitRepositoryDto[]> {
    return this.get<GitRepositoryDto[]>(`/api/git/repositories?projectId=${projectId}`);
  }

  /**
   * Scan project directory for .git folders and sync to database
   */
  scanProjectForGitRepos(projectId: number): Observable<GitRepositoryDto[]> {
    return this.get<GitRepositoryDto[]>(`/api/git/repositories/scan?projectId=${projectId}`);
  }

  /**
   * Get cached Git branches for a repository from database
   */
  getGitBranches(gitRepositoryId: number): Observable<GitBranchDto[]> {
    return this.get<GitBranchDto[]>(`/api/branches?gitRepositoryId=${gitRepositoryId}`);
  }

  /**
   * Scan repository using LibGit2Sharp and sync branches to database
   */
  scanGitRepoBranches(gitRepositoryId: number): Observable<GitBranchDto[]> {
    return this.get<GitBranchDto[]>(`/api/branches/scan?gitRepositoryId=${gitRepositoryId}`);
  }
}