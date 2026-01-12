import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { UserDto, LoginRequest, RegisterRequest } from '../../shared/models/api.models';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private baseUrl = `${environment.apiUrl}/api/users`;

  constructor(private http: HttpClient) {}

  register(request: RegisterRequest): Observable<{ success: boolean; data: UserDto }> {
    return this.http.post<{ success: boolean; data: UserDto }>(`${this.baseUrl}/register`, request);
  }

  login(request: LoginRequest): Observable<{ success: boolean; data: UserDto }> {
    return this.http.post<{ success: boolean; data: UserDto }>(`${this.baseUrl}/login`, request);
  }

  getUserById(id: number): Observable<{ success: boolean; data: UserDto }> {
    return this.http.get<{ success: boolean; data: UserDto }>(`${this.baseUrl}/${id}`);
  }

  getAllUsers(): Observable<{ success: boolean; data: UserDto[] }>{
    return this.http.get<{ success: boolean; data: UserDto[] }>(`${this.baseUrl}`);
  }
}