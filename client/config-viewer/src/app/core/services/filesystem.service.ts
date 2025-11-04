import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { 
  FileSystemNode, 
  FileSystemSearchParams, 
  PagedResult 
} from '../../shared/models/api.models';

@Injectable({
  providedIn: 'root'
})
export class FileSystemService extends ApiService {
  
  searchFileSystem(params: FileSystemSearchParams): Observable<PagedResult<FileSystemNode>> {
    const httpParams = this.buildParams(params);
    return this.get<PagedResult<FileSystemNode>>('/api/filesystem/search', httpParams);
  }

  getFileSystemNode(projectId: number, fileSystemNodeId: number): Observable<any> {
    const params = this.buildParams({ fileSystemNodeId });
    return this.get<any>(`/api/filesystem/${projectId}`, params);
  }

  createFile(projectId: number, relativePath: string, content: string): Observable<any> {
    const params = this.buildParams({ relativePath, content });
    return this.post<any>(`/api/filesystem/createfile/${projectId}`, null, params);
  }

  createFolder(projectId: number, relativePath: string): Observable<any> {
    const params = this.buildParams({ relativePath });
    return this.post<any>(`/api/filesystem/createfolder/${projectId}`, null, params);
  }

  updateFile(projectId: number, fileSystemNodeId: number, content: string): Observable<any> {
    const params = this.buildParams({ fileSystemNodeId, content });
    return this.post<any>(`/api/filesystem/updatefile/${projectId}`, null, params);
  }

  syncFileSystem(projectId: number): Observable<any> {
    return this.post<any>(`/api/filesystem/sync/${projectId}`);
  }
}
