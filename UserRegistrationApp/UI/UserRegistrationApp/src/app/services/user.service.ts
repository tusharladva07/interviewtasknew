import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import { environment } from '../environment';
import { UserDetail, UserFormValue, UserListItem } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly listRefreshSource = new Subject<void>();
  readonly listRefresh$ = this.listRefreshSource.asObservable();

  constructor(private http: HttpClient) { }

  requestListRefresh(): void {
    this.listRefreshSource.next();
  }

  getUserList(): Observable<UserListItem[]> {
    return this.http.get<UserListItem[]>(`${environment.apiUrl}/User/GetList`);
  }

  getUserDetails(id: number): Observable<UserDetail> {
    return this.http.get<UserDetail>(`${environment.apiUrl}/User/GetDetails/${id}`);
  }

  addUser(userDto: UserFormValue): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${environment.apiUrl}/User/Create`, userDto);
  }

  updateUser(id: number, user: UserFormValue): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${environment.apiUrl}/User/Update/${id}`, user);
  }

  deleteUser(id: number): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${environment.apiUrl}/User/Delete/${id}`, null);
  }
}
