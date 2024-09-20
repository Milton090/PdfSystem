import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environment/environment';
import { AuthI } from '../interfaces/auth.interface';
import { ResponseI } from '../interfaces/response.interface';
import { Observable } from 'rxjs';


@Injectable({
  providedIn: 'root'
})

export class AuthService {

  private readonly baseUrl: string = `${environment.apiUrl}${environment.authController}`;

  constructor(private http: HttpClient) {}

  login(user: AuthI): Observable<ResponseI> {
    return this.http.post<ResponseI>(`${this.baseUrl}/login`, user);
  }

  register(user: AuthI): Observable<ResponseI> {
    return this.http.post<ResponseI>(`${this.baseUrl}/register`, user);
  }
}