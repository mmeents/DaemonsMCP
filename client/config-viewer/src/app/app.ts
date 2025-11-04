import { Component, signal, OnInit, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ProjectsService } from './core/services/projects.service';
import { Project } from './shared/models/api.models';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {
  protected readonly title = signal('DaemonsMCP Config Viewer');
  protected projects = signal<Project[]>([]);
  protected loading = signal(true);
  protected error = signal<string | null>(null);

  private projectsService = inject(ProjectsService);

  ngOnInit() {
    this.loadProjects();
  }

  loadProjects() {
    this.loading.set(true);
    this.error.set(null);
    
    this.projectsService.getProjects().subscribe({
      next: (projects) => {
        this.projects.set(projects);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading projects:', err);
        this.error.set('Failed to load projects. Make sure the API is running at https://localhost:44356');
        this.loading.set(false);
      }
    });
  }
}
