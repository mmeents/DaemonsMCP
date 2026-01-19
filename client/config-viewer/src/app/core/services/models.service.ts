import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { ApiService } from './api.service';
import { 
  ModelDto, 
  ModelTypeDto, 
  ModelPropertyDto,
  AddUpdateModelRequest,
  AddUpdateModelPropertyRequest,
  SearchModelsParams,
  TemplateExecutionResult,
  ApiResponse,
  ExecuteTemplateCommand
} from '../../shared/models/api.models';

@Injectable({
  providedIn: 'root'
})
export class ModelsService extends ApiService {
  
  // Search models with optional filters
  searchModels(params: SearchModelsParams): Observable<ModelDto[]> {
    const httpParams = this.buildParams({
      projectId: params.projectId,
      parentId: params.parentId,
      modelTypeId: params.modelTypeId,
      nameFilter: params.nameFilter,
      maxDepth: params.maxDepth ?? 1,
      includeProperties: params.includeProperties ?? false
    });
    return this.get<ModelDto[]>('/api/models/search', httpParams);
  }

  // Get model by ID
  getModelById(modelId: number, maxDepth: number = 1, includeProperties: boolean = true): Observable<ModelDto> {
    const httpParams = this.buildParams({ maxDepth, includeProperties });
    return this.get<ModelDto>(`/api/models/${modelId}`, httpParams);
  }

  // Add or update model
  addUpdateModel(request: AddUpdateModelRequest): Observable<ModelDto> {
    return this.post<ModelDto>('/api/models', request);
  }

  // Delete model
  deleteModel(modelId: number): Observable<{success: boolean}> {
    return this.delete<{success: boolean}>(`/api/models/${modelId}`);
  }

  // Get model properties
  getModelProperties(modelId: number): Observable<ModelPropertyDto[]> {
    return this.get<ModelPropertyDto[]>(`/api/models/${modelId}/properties`);
  }

  // Add or update model property
  addUpdateModelProperty(request: AddUpdateModelPropertyRequest): Observable<ModelPropertyDto> {
    return this.post<ModelPropertyDto>('/api/models/properties', request);
  }

  // Add this method to ModelsService
  deleteModelProperty(propertyId: number): Observable<{success: boolean}> {
    return this.delete<{success: boolean}>(`/api/models/properties/${propertyId}`);
  }

  // Get editor types
  getEditorTypes(): Observable<ModelTypeDto[]> {
    return this.get<ModelTypeDto[]>('/api/modeltypes/editors');
  }

  // Get SQL data types
  getSqlDataTypes(): Observable<ModelTypeDto[]> {
    return this.get<ModelTypeDto[]>('/api/modeltypes/sqltypes');
  }

  // Get project templates
  getProjectTemplates(): Observable<ModelTypeDto[]> {
    return this.get<ModelTypeDto[]>('/api/modeltypes/templates');
  }

  getValidChildTypes(parentModelTypeId: number): Observable<ModelTypeDto[]> {
    return this.get<ModelTypeDto[]>(`/api/modeltypes/${parentModelTypeId}/valid-children`);    
  }

  executeTemplate(command: ExecuteTemplateCommand): Observable<TemplateExecutionResult> {
    return this.post<TemplateExecutionResult>(`/api/templates/${command.templateId}/execute`,
      { targetModelId: command.targetModelId, saveToFile: command.saveToFile }
    );
  }

  importTable(request: { parentId: number; sqlStatement: string }) {
    return this.http.post<ModelDto[]>(`${this.apiUrl}/api/models/import-table`, request);
  }
}