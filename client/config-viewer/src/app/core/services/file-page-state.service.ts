import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class FilePageStateService {
  private readonly PROJECT_KEY = 'files_projectId';
  private readonly SEARCH_KEY = 'files_searchTerm';
  private readonly FILE_KEY = 'files_selectedFileId';
  
  setProjectId(id: number): void {
    sessionStorage.setItem(this.PROJECT_KEY, id.toString());
  }
  
  getProjectId(): number | null {
    const stored = sessionStorage.getItem(this.PROJECT_KEY);
    return stored ? parseInt(stored, 10) : null;
  }
  
  setSearchTerm(term: string): void {
    sessionStorage.setItem(this.SEARCH_KEY, term);
  }
  
  getSearchTerm(): string {
    return sessionStorage.getItem(this.SEARCH_KEY) || '';
  }
  
  setSelectedFileId(id: number | null): void {
    if (id === null) {
      sessionStorage.removeItem(this.FILE_KEY);
    } else {
      sessionStorage.setItem(this.FILE_KEY, id.toString());
    }
  }
  
  getSelectedFileId(): number | null {
    const stored = sessionStorage.getItem(this.FILE_KEY);
    return stored ? parseInt(stored, 10) : null;
  }
  
  clear(): void {
    sessionStorage.removeItem(this.PROJECT_KEY);
    sessionStorage.removeItem(this.SEARCH_KEY);
    sessionStorage.removeItem(this.FILE_KEY);
  }
}