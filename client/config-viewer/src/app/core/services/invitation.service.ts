import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import {
  InvitationTokenDto,
  InvitationTokenSearchParams,
  CreateInvitationCommand,
  RegisterWithInvitationCommand,
  ApiResponse,
  PagedResult,
  UserDto
} from '../../shared/models/api.models';

@Injectable({
  providedIn: 'root'
})
export class InvitationService {
  private readonly apiUrl = `${environment.apiUrl}/api/invitations`;

  constructor(private http: HttpClient) {}

  validateInvitationToken(token: string): Observable<InvitationTokenDto>{
    const params = new HttpParams().set('token', token);
    return this.http.get<ApiResponse<InvitationTokenDto>>(`${this.apiUrl}/validate`, { params })
      .pipe(map(response => response.data));
  }

  searchInvitations(params: InvitationTokenSearchParams): Observable<PagedResult<InvitationTokenDto>> {
    let httpParams = new HttpParams();
    
    if (params.invitedEmail) httpParams = httpParams.set('invitedEmail', params.invitedEmail);
    if (params.includeExpired !== undefined) httpParams = httpParams.set('includeExpired', params.includeExpired);
    if (params.includeUsed !== undefined) httpParams = httpParams.set('includeUsed', params.includeUsed);
    if (params.pageNo) httpParams = httpParams.set('pageNo', params.pageNo);
    if (params.pageSize) httpParams = httpParams.set('pageSize', params.pageSize);

    return this.http.get<ApiResponse<PagedResult<InvitationTokenDto>>>(`${this.apiUrl}/search`, { params: httpParams })
      .pipe(map(response => response.data));
  }

  getInvitationById(id: number): Observable<InvitationTokenDto> {
    return this.http.get<ApiResponse<InvitationTokenDto>>(`${this.apiUrl}/${id}`)
      .pipe(map(response => response.data));
  }

  createInvitation(command: CreateInvitationCommand): Observable<InvitationTokenDto> {
    return this.http.post<ApiResponse<InvitationTokenDto>>(this.apiUrl, command)
      .pipe(map(response => response.data));
  }

  registerWithInvitation(command: RegisterWithInvitationCommand): Observable<UserDto> {
    return this.http.post<ApiResponse<UserDto>>(`${this.apiUrl}/register`, command)
      .pipe(map(response => response.data));
  }

  revokeInvitation(id: number): Observable<void> {
    return this.http.post<ApiResponse<void>>(`${this.apiUrl}/${id}/revoke`, {})
      .pipe(map(response => response.data));
  }
}