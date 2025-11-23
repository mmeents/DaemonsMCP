import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpParams } from '@angular/common/http';
import { ApiService } from './api.service';
import { 
  ItemDto, 
  ItemTypeDto, 
  AddUpdateItemRequest, 
  AddUpdateItemTypeRequest, 
  SearchItemsParams 
} from '../../shared/models/api.models';

@Injectable({
  providedIn: 'root'
})
export class ItemsService extends ApiService {
  
  // Search items with optional filters
  searchItems(params: SearchItemsParams): Observable<ItemDto[]> {
    const httpParams = this.buildParams({
      parentId: params.parentId,
      nameContains: params.nameContains,
      detailsContains: params.detailsContains,
      typeId: params.typeId,
      statusId: params.statusId,
      maxDepth: params.maxDepth ?? 1
    });
    return this.get<ItemDto[]>('/api/items/search', httpParams);
  }

  // Get item by ID
  getItemById(itemId: number, maxDepth: number = 1): Observable<ItemDto> {
    const httpParams = this.buildParams({ maxDepth });
    return this.get<ItemDto>(`/api/items/${itemId}`, httpParams);
  }

  // Add or update item
  addUpdateItem(request: AddUpdateItemRequest): Observable<ItemDto> {
    return this.post<ItemDto>('/api/items', request);
  }

  // Get all item types (both regular and status types)
  getAllItemTypes(): Observable<ItemTypeDto[]> {
    return this.get<ItemTypeDto[]>('/api/items/types/all');
  }

  // Get item types (non-status types only)
  getItemTypes(): Observable<ItemTypeDto[]> {
    return this.get<ItemTypeDto[]>('/api/items/types');
  }

  // Get status types
  getStatusTypes(): Observable<ItemTypeDto[]> {
    return this.get<ItemTypeDto[]>('/api/items/status-types');
  }

  // Add or update item type
  addUpdateItemType(request: AddUpdateItemTypeRequest): Observable<ItemTypeDto> {
    return this.post<ItemTypeDto>('/api/items/types', request);
  }
}