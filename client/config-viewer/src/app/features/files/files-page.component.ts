import { Component, signal, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProjectsService } from '../../core/services/projects.service';
import { FileSystemService } from '../../core/services/filesystem.service';
import { Project, FileSystemNode } from '../../shared/models/api.models';
import { ColumnConfig } from '../../shared/components/searchable-list/searchable-list.component';
import { FileViewerComponent } from '../../shared/components/file-viewer/file-viewer.component';
import { FilePageStateService } from '../../core/services/file-page-state.service';

@Component({
  selector: 'app-files-page',
  standalone: true,
  imports: [CommonModule, FileViewerComponent],
  templateUrl: './files-page.component.html',
  styleUrl: './files-page.component.scss',
})
export class FilesPageComponent implements OnInit {
  protected projects = signal<Project[]>([]);
  protected selectedProject = signal<Project | null>(null);
  protected files = signal<FileSystemNode[]>([]);
  protected loading = signal(true);
  protected searchLoading = signal(false);
  protected error = signal<string | null>(null);
  protected selectedFileName = signal<string>('');
  protected selectedFilePath = signal<string>('');
  protected fileContent = signal<string>('');
  protected fileLoading = signal<boolean>(false);
  protected fileError = signal<string | null>(null);
  protected searchTerm = signal<string>('');
  protected showResults = signal<boolean>(false);
  protected selectedFileId = signal<number | null>(null);

  private stateService = inject(FilePageStateService);
  private projectsService = inject(ProjectsService);
  private fileSystemService = inject(FileSystemService);

  // Column configuration for file list
  protected fileColumns: ColumnConfig<FileSystemNode>[] = [
    {
      header: 'Name',
      width: '30%',
      getValue: (file) => file.name
    },
    {
      header: 'Path',
      width: '50%',
      getValue: (file) => file.relativePath
    },
    {
      header: 'Size',
      width: '10%',
      align: 'right',
      getValue: (file) => this.formatSize(file.sizeInBytes || 0)
    },
    {
      header: 'ProjectId',
      width: '10%',
      align: 'right',
      getValue: (file) => file.projectId.toString()
    }
  ];

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
        
        // Try to restore saved project first
        const savedProjectId = this.stateService.getProjectId();
        if (savedProjectId) {
          const savedProject = projects.find(p => p.id === savedProjectId);
          if (savedProject) {
            this.selectProject(savedProject);
            
            // If there was a search term, restore the search
            const savedSearchTerm = this.stateService.getSearchTerm();
            if (savedSearchTerm) {
              this.onSearchChanged(savedSearchTerm);
            }
            
            // Try to restore selected file
            const savedFileId = this.stateService.getSelectedFileId();
            if (savedFileId) {
              this.restoreSelectedFile(savedFileId);
            }
            
            return;
          }
        }
        
        // Fallback: Auto-select first project if no saved state
        if (projects.length > 0) {
          this.selectProject(projects[0]);
        }
      },
      error: (err) => {
        console.error('Error loading projects:', err);
        this.error.set('Failed to load projects. Make sure the API is running at https://localhost:44356');
        this.loading.set(false);
      }
    });
  }

  selectProject(project: Project) {
    this.selectedProject.set(project);
    this.stateService.setProjectId(project.id);
    this.files.set([]); 
      
    this.selectedFileId.set(null);
    this.stateService.setSelectedFileId(null);
  }

  onSearchChanged(searchTerm: string) {
    const project = this.selectedProject();
    if (!project) return;

    // Empty search = clear results
    if (!searchTerm.trim()) {
      this.files.set([]);
      return;
    }

    this.searchLoading.set(true);
    
    this.fileSystemService.searchFileSystem({
      projectId: project.id,
      filter: searchTerm,
      includeDirectories: false,
      includeFiles: true,
      pageNo: 1,
      pageSize: 50
    }).subscribe({
      next: (files) => {        
        this.files.set(files as any);
        this.searchLoading.set(false);
      },
      error: (err) => {
        console.error('Error searching files:', err);
        this.searchLoading.set(false);
      }
    });
  }

  onSearchInput(value: string) {
    this.searchTerm.set(value);
    this.stateService.setSearchTerm(value);
    this.showResults.set(true);
    
    if (!value.trim()) {
        this.files.set([]);
        return;
    }

    // Debounce logic would go here (or use RxJS Subject)
    this.onSearchChanged(value);
  }

  // Add these focus handlers:
  onSearchFocus() {
    if (this.searchTerm() && this.files().length > 0) {
        this.showResults.set(true);
    }
  }

  onSearchBlur() {
    // Delay to allow click on result
    setTimeout(() => {
        this.showResults.set(false);
    }, 200);
  }

  clearSearch() {
    this.searchTerm.set('');
    this.stateService.setSearchTerm('');
    this.files.set([]);
    this.showResults.set(false);
  }

    // Update onFileSelected:
  onFileSelected(file: any) {
    console.log('Selected file:', file);
    
    this.selectedFileId.set(file.id);
    this.stateService.setSelectedFileId(file.id);
    this.selectedFileName.set(file.name);
    this.selectedFilePath.set(file.relativePath);
    this.fileLoading.set(true);
    this.fileError.set(null);
    this.showResults.set(false); // Hide dropdown after selection
    
    const project = this.selectedProject();
    if (!project) return;

    this.fileSystemService.getFileSystemNode(project.id, file.id).subscribe({
        next: (value) => {
        console.log('File content response:', value);
        this.fileContent.set(value.content || '');
        this.fileLoading.set(false);
        },
        error: (err) => {
        console.error('Error loading file:', err);
        this.fileError.set('Failed to load file content');
        this.fileLoading.set(false);
        }
    });
  }

  private restoreSelectedFile(fileId: number) {
    const project = this.selectedProject();
    if (!project) return;
    
    this.fileSystemService.getFileSystemNode(project.id, fileId).subscribe({
      next: (fileNode) => {
        // Simulate file selection
        this.onFileSelected({
          project: fileNode.projectId,
          id: fileNode.fileSystemNodeId,  
          name: fileNode.name,        
          relativePath: fileNode.relativePath
        });
      },
      error: (err) => {
        console.error('Error restoring file:', err);
        // Clear invalid file ID
        this.stateService.setSelectedFileId(null);
      }
    });
  }

  private formatSize(bytes: number): string {
    if (bytes === 0) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round((bytes / Math.pow(k, i)) * 100) / 100 + ' ' + sizes[i];
  }
}