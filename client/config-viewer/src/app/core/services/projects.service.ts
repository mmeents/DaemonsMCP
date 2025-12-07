import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Project, CreateProjectCommand } from '../../shared/models/api.models';

@Injectable({
  providedIn: 'root'
})
export class ProjectsService extends ApiService {
  
  getProjects(): Observable<Project[]> {
    return this.get<Project[]>('/api/projects');
  }

  getProject(id: number): Observable<Project> {
    return this.get<Project>(`/api/projects/${id}`);
  }

  createProject(command: CreateProjectCommand): Observable<Project> {
    return this.post<Project>('/api/projects', command);
  }
  
  updateProject(id: number, command: CreateProjectCommand): Observable<Project> {
    return this.put<Project>(`/api/projects/${id}`, command);
  }

  deleteProject(id: number): Observable<void> {
    return this.delete<void>(`/api/projects/${id}`);
  }

  getReadme(): Observable<any> {
    return this.get<any>('/api/readme');
  }

  checkHealth(): Observable<any> {
    return this.get<any>('/health');
  }
}
