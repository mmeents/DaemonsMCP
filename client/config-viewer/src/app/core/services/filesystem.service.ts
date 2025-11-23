import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from './api.service';
import { 
  FileSystemNode, 
  FileSystemSearchParams, 
  PagedResult,
  ApiResponse, 
  FileSystemFile
} from '../../shared/models/api.models';

@Injectable({
  providedIn: 'root'
})
export class FileSystemService extends ApiService {
  
  searchFileSystem(params: FileSystemSearchParams): Observable<PagedResult<FileSystemNode>> {
    const httpParams = this.buildParams(params);
    return this.get<ApiResponse<PagedResult<FileSystemNode>>>('/api/filesystem/search', httpParams)
      .pipe(
        map(response => response.data) 
      );
  }

  getFileSystemNode(projectId: number, fileSystemNodeId: number): Observable<FileSystemFile> {
    const params = this.buildParams({ fileSystemNodeId });
    return this.get<FileSystemFile>(`/api/filesystem/${projectId}`, params)
      .pipe(
        map(response => response) 
      );
  }

  createFile(projectId: number, relativePath: string, content: string): Observable<any> {
    const params = this.buildParams({ relativePath, content });
    return this.post<ApiResponse<any>>(`/api/filesystem/createfile/${projectId}`, null, params)
      .pipe(
        map(response => response.data) 
      );
  }

  createFolder(projectId: number, relativePath: string): Observable<any> {
    const params = this.buildParams({ relativePath });
    return this.post<ApiResponse<any>>(`/api/filesystem/createfolder/${projectId}`, null, params)
      .pipe(
        map(response => response.data)  // Capital D
      );
  }

  updateFile(projectId: number, fileSystemNodeId: number, content: string): Observable<any> {
    const params = this.buildParams({ fileSystemNodeId, content });
    return this.post<ApiResponse<any>>(`/api/filesystem/updatefile/${projectId}`, null, params)
      .pipe(
        map(response => response.data)  // Capital D
      );
  }

  syncFileSystem(projectId: number): Observable<any> {
    return this.post<ApiResponse<any>>(`/api/filesystem/sync/${projectId}`)
      .pipe(
        map(response => response.data)  // Capital D
      );
  }
}