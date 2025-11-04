import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { IndexingQueueStatus } from '../../shared/models/api.models';

@Injectable({
  providedIn: 'root'
})
export class IndexingService extends ApiService {
  
  runIndexing(projectId?: number): Observable<any> {
    const params = projectId ? this.buildParams({ projectId }) : undefined;
    return this.post<any>('/api/indexing/run', null, params);
  }

  getQueueStatus(projectId?: number): Observable<IndexingQueueStatus> {
    const params = projectId ? this.buildParams({ projectId }) : undefined;
    return this.get<IndexingQueueStatus>('/api/indexing/queue/status', params);
  }
}
