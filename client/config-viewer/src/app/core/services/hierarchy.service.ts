import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { 
  ObjectHierarchy, 
  ObjectHierarchySearchParams, 
  PagedResult 
} from '../../shared/models/api.models';

@Injectable({
  providedIn: 'root'
})
export class HierarchyService extends ApiService {
  
  searchObjectHierarchy(params: ObjectHierarchySearchParams): Observable<PagedResult<ObjectHierarchy>> {
    const httpParams = this.buildParams(params);
    return this.get<PagedResult<ObjectHierarchy>>('/api/hierarchy/search', httpParams);
  }
}
