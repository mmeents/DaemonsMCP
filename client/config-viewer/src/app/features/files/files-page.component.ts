import { Component, signal, OnInit, inject, effect } from '@angular/core';
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
  protected quickJumpId = signal<number | null>(null);

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

  constructor() {
    // Sync the ID input field whenever selectedFileId changes from other sources
    effect(() => {
      const fileId = this.selectedFileId();
      if (fileId !== null && fileId !== this.quickJumpId()) {
        this.quickJumpId.set(fileId);
      }
    });
  }

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

  onQuickJumpById(idValue: string) {
    console.log('=== Quick Jump Called ===');
    console.log('Input value:', idValue);
    
    const trimmed = idValue.trim();
    
    if (!trimmed) {
      console.log('Empty input, clearing');
      this.quickJumpId.set(null);
      return;
    }

    const id = parseInt(trimmed, 10);
    console.log('Parsed ID:', id);
    
    if (isNaN(id) || id <= 0) {
      console.warn('Invalid ID');
      this.fileError.set(`Invalid ID: ${idValue}`);
      return;
    }

    const project = this.selectedProject();
    if (!project) {
      console.warn('No project selected');
      return;
    }

    if (id === this.selectedFileId()) {
      console.log('Already selected, skipping');
      return;
    }

    console.log(`Fetching file ${id} from project ${project.id}`);
    
    this.quickJumpId.set(id);
    this.fileLoading.set(true);
    this.fileError.set(null);

    this.fileSystemService.getFileSystemNode(project.id, id).subscribe({
      next: (fileNode) => {
        console.log('=== API Response ===');
        console.log('Full response:', fileNode);
        const fileName = this.extractFileName(fileNode.relativePath);
        console.log('name:', fileName);
        console.log('relativePath:', fileNode.relativePath);
        console.log('content length:', fileNode.content?.length);
        console.log('fileSystemNodeId:', fileNode.fileSystemNodeId);
        
        // Set fileName FIRST before content
        console.log('Setting fileName to:', fileName);
        this.selectedFileName.set(fileName);
        
        console.log('Setting filePath to:', fileNode.relativePath);
        this.selectedFilePath.set(fileNode.relativePath);
        
        console.log('Setting fileId to:', fileNode.fileSystemNodeId);
        this.selectedFileId.set(fileNode.fileSystemNodeId);
        this.stateService.setSelectedFileId(fileNode.fileSystemNodeId);
        
        // Set content LAST
        console.log('Setting content, length:', fileNode.content?.length);
        this.fileContent.set(fileNode.content || '');
        
        console.log('Setting loading to false');
        this.fileLoading.set(false);
        
        // Clear search
        this.searchTerm.set('');
        this.stateService.setSearchTerm('');
        this.files.set([]);
        this.showResults.set(false);
        
        console.log('=== Quick Jump Complete ===');
      },
      error: (err) => {
        console.error('=== API Error ===', err);
        this.fileError.set(`File with ID ${id} not found in project "${project.name}"`);
        this.fileLoading.set(false);
        this.quickJumpId.set(null);
      }
    });
  }

    // Update onFileSelected:
  onFileSelected(file: any) {
    console.log('Selected file:', file);
    
    this.selectedFileId.set(file.id);
    this.stateService.setSelectedFileId(file.id);
    this.selectedFileName.set(file.name);
    this.selectedFilePath.set(file.relativePath);
    this.quickJumpId.set(file.id);
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

  private extractFileName(relativePath: string): string {
    // Handle both forward and back slashes
    const parts = relativePath.split(/[/\\]/);
    return parts[parts.length - 1] || 'unknown';
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