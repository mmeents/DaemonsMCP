import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { 
  AccessTokenDto, 
  AccessTokenSearchParams,
  CreateAccessTokenCommand,
  ApiResponse,
  PagedResult 
} from '../../shared/models/api.models';

@Injectable({
  providedIn: 'root'
})
export class AccessTokensService {
  private readonly apiUrl = `${environment.apiUrl}/api/tokens`;

  constructor(private http: HttpClient) {}

  searchTokens(params: AccessTokenSearchParams): Observable<PagedResult<AccessTokenDto>> {
    let httpParams = new HttpParams();
    
    if (params.issuedTo) httpParams = httpParams.set('issuedTo', params.issuedTo);
    if (params.includeExpired !== undefined) httpParams = httpParams.set('includeExpired', params.includeExpired);
    if (params.includeUsed !== undefined) httpParams = httpParams.set('includeUsed', params.includeUsed);
    if (params.pageNo) httpParams = httpParams.set('pageNo', params.pageNo);
    if (params.pageSize) httpParams = httpParams.set('pageSize', params.pageSize);

    return this.http.get<ApiResponse<PagedResult<AccessTokenDto>>>(`${this.apiUrl}/search`, { params: httpParams })
      .pipe(map(response => response.data));
  }

  getTokenById(id: number): Observable<AccessTokenDto> {
    return this.http.get<ApiResponse<AccessTokenDto>>(`${this.apiUrl}/${id}`)
      .pipe(map(response => response.data));
  }

  createToken(command: CreateAccessTokenCommand): Observable<AccessTokenDto> {
    return this.http.post<ApiResponse<AccessTokenDto>>(this.apiUrl, command)
      .pipe(map(response => response.data));
  }

  revokeToken(id: number): Observable<void> {
    return this.http.post<ApiResponse<void>>(`${this.apiUrl}/${id}/revoke`, {})
      .pipe(map(response => response.data));
  }
}