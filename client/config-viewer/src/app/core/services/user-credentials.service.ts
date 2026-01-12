import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { 
  UserCredentialDto, 
  CreateUserCredentialCommand,
  UpdateUserCredentialCommand
} from '../../shared/models/api.models';

@Injectable({
  providedIn: 'root'
})
export class UserCredentialsService extends ApiService {
  
  /**
   * Get all credentials for a user
   */
  getUserCredentials(userId: number): Observable<UserCredentialDto[]> {
    return this.get<UserCredentialDto[]>(`/api/user-credentials?userId=${userId}`);
  }

  /**
   * Create a new credential
   */
  createCredential(command: CreateUserCredentialCommand): Observable<UserCredentialDto> {
    return this.post<UserCredentialDto>('/api/user-credentials', command);
  }

  /**
   * Update an existing credential
   */
  updateCredential(id: number, command: UpdateUserCredentialCommand): Observable<UserCredentialDto> {
    return this.put<UserCredentialDto>(`/api/user-credentials/${id}`, command);
  }

  /**
   * Delete a credential
   */
  deleteCredential(id: number): Observable<void> {
    return this.delete<void>(`/api/user-credentials/${id}`);
  }
}