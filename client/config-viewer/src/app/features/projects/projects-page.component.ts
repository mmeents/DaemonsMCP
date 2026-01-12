// client/config-viewer/src/app/features/projects/projects-page.component.ts

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProjectsTreeComponent } from '../../shared/components/projects-tree/projects-tree.component';
import { ProjectsService } from '../../core/services/projects.service';
import { GitService } from '../../core/services/git.service';
import { Project, CreateProjectCommand, GitRepositoryDto } from '../../shared/models/api.models';

import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { TabsModule } from 'primeng/tabs';
import { CardModule } from 'primeng/card';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { MessageModule } from 'primeng/message';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-projects-page',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    ProjectsTreeComponent, 
    DialogModule, 
    ButtonModule,
    InputTextModule,
    TextareaModule,
    TabsModule,
    CardModule,
    ProgressSpinnerModule,
    MessageModule,
    ToastModule
  ],
  providers: [MessageService],
  templateUrl: './projects-page.component.html',
  styleUrl: './projects-page.component.scss',
})
export class ProjectsPageComponent implements OnInit {
  projects: Project[] = [];
  selectedProject?: Project;
  loading = false;

  isEditing = false;
  isSaving = false;
  editForm: CreateProjectCommand & { id?: number } = this.createEmptyForm();

  showDeleteDialog = false;
  projectToDelete?: Project;
  gitRepos: GitRepositoryDto[] = [];
  loadingGitRepos = false;
  scanningGitRepos = false;

  constructor(
    private projectsService: ProjectsService,
    private gitService: GitService,
    private messageService: MessageService
  ) {}

  ngOnInit() {
    this.loadProjects();
  }

  private createEmptyForm(): CreateProjectCommand & { id?: number } {
    return {
      id: undefined,
      name: '',
      description: '',
      rootPath: ''
    };
  }

  loadProjects() {
    this.loading = true;
    this.projectsService.getProjects().subscribe({
      next: (projects) => {
        this.projects = projects;
        this.loading = false;
        console.log('Loaded projects:', projects);
      },
      error: (err) => {
        console.error('Failed to load projects:', err);
        this.loading = false;
      }
    });
  }

  onProjectSelected(project: Project) {
    this.selectedProject = project;
    console.log('Selected project:', project);
    this.loadGitRepos();
  }

  loadGitRepos() {
    if (!this.selectedProject) {
      this.gitRepos = [];
      return;
    }

    this.loadingGitRepos = true;
    this.gitService.getGitReposForProject(this.selectedProject.id).subscribe({
      next: (repos) => {
        this.gitRepos = repos;
        this.loadingGitRepos = false;
        console.log('Loaded Git repos:', repos);
      },
      error: (err) => {
        console.error('Failed to load Git repos:', err);
        this.loadingGitRepos = false;
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to load Git repositories'
        });
      }
    });
  }

   onScanForGitRepos() {
    if (!this.selectedProject) return;

    this.scanningGitRepos = true;
    this.gitService.scanProjectForGitRepos(this.selectedProject.id).subscribe({
      next: (repos) => {
        this.gitRepos = repos;
        this.scanningGitRepos = false;
        this.messageService.add({
          severity: 'success',
          summary: 'Scan Complete',
          detail: `Found ${repos.length} Git ${repos.length === 1 ? 'repository' : 'repositories'}`
        });
        console.log('Scanned Git repos:', repos);
      },
      error: (err) => {
        console.error('Failed to scan for Git repos:', err);
        this.scanningGitRepos = false;
        this.messageService.add({
          severity: 'error',
          summary: 'Scan Failed',
          detail: err.error?.errorMessage || 'Failed to scan for Git repositories'
        });
      }
    });
  }

  getStatusClass(repo: GitRepositoryDto): string {
    return repo.isDirty ? 'status-dirty' : 'status-clean';
  }

  getStatusIcon(repo: GitRepositoryDto): string {
    return repo.isDirty ? 'pi pi-exclamation-triangle' : 'pi pi-check-circle';
  }

  getStatusText(repo: GitRepositoryDto): string {
    if (!repo.isDirty) {
      return 'Clean';
    }
    
    const parts: string[] = [];
    if (repo.modifiedFileCount && repo.modifiedFileCount > 0) {
      parts.push(`${repo.modifiedFileCount} modified`);
    }
    if (repo.untrackedFileCount && repo.untrackedFileCount > 0) {
      parts.push(`${repo.untrackedFileCount} untracked`);
    }
    
    return parts.length > 0 ? parts.join(', ') : 'Dirty';
  }

  onNewProject() {
    this.editForm = this.createEmptyForm();
    this.isEditing = true;
  }

  onEditProject(project?: Project) {
    const projectToEdit = project ?? this.selectedProject;
    if (!projectToEdit) return;
    
    this.editForm = {
      id: projectToEdit.id,
      name: projectToEdit.name,
      description: projectToEdit.description,
      rootPath: projectToEdit.rootPath
    };

    this.isEditing = true;
  }

  onDeleteProject(project: Project) {
    this.projectToDelete = project;
    this.showDeleteDialog = true;
  }

  confirmDelete() {
    if (!this.projectToDelete) return;
    
    this.projectsService.deleteProject(this.projectToDelete.id).subscribe({
      next: () => {
        console.log('Project deleted successfully');
        this.selectedProject = undefined;
        this.showDeleteDialog = false;
        this.loadProjects();
      },
      error: (err) => {
        console.error('Failed to delete project:', err);
        alert('Failed to delete project. ' + (err.error || 'Check console for details.'));
        this.showDeleteDialog = false;
      }
    });
  }

  cancelDelete() {
    this.showDeleteDialog = false;
    this.projectToDelete = undefined;
  }

  onSaveProject() {
    if (!this.editForm.name.trim()) {
      alert('Name is required');
      return;
    }

    if (!this.editForm.rootPath.trim()) {
      alert('Root Path is required');
      return;
    }
    
    this.isSaving = true;
    
    const command: CreateProjectCommand = {
      name: this.editForm.name,
      description: this.editForm.description,
      rootPath: this.editForm.rootPath
    };

    const saveOperation = this.editForm.id
      ? this.projectsService.updateProject(this.editForm.id, command)
      : this.projectsService.createProject(command);

    saveOperation.subscribe({
      next: (savedProject) => {
        console.log('Project saved:', savedProject);
        this.isSaving = false;
        this.isEditing = false;
        this.selectedProject = savedProject;
        this.loadProjects();
      },
      error: (err) => {
        console.error('Failed to save project:', err);
        this.isSaving = false;
        alert('Failed to save project. Check console for details.');
      }
    });
  }

  onCancelEdit() {
    this.isEditing = false;
    if (!this.editForm.id) {
      this.selectedProject = undefined;
    }
  }
}