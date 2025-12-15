// client/config-viewer/src/app/features/projects/projects-page.component.ts

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProjectsTreeComponent } from '../../shared/components/projects-tree/projects-tree.component';
import { ProjectsService } from '../../core/services/projects.service';
import { Project, CreateProjectCommand } from '../../shared/models/api.models';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';

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
    TextareaModule
  ],
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

  constructor(private projectsService: ProjectsService) {}

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