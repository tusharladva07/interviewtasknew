import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private http: HttpClient) { }
login(user: any) {
    return this.http.post(`${environment.apiUrl}Auth/login`, user);
  }
  getToken()
  {
    return localStorage.getItem('token');
  }
   setToken(token: string)
  {
    localStorage.setItem('token', token);
  }
  logout()
  {
    localStorage.removeItem("token"); 
  }
}
