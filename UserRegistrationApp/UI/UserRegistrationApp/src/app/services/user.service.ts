import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environment';

@Injectable({
  providedIn: 'root'
})
export class UserService {
constructor(private http: HttpClient) { }

GetUserList(){
  return this.http.get(`${environment.apiUrl}/User/GetList`);
}
GetUserDetails(id: number){
  return this.http.get(`${environment.apiUrl}/User/GetDetails/${id}`);
}
AddUser(userDto: any){
  return this.http.post(`${environment.apiUrl}/User/Create`, userDto);
}
updateUser(id: number, user: any){
  return this.http.put(`${environment.apiUrl}/User/Update/${id}`, user);
}
deleteUser(id: number){
  return this.http.post(`${environment.apiUrl}/User/Delete/${id}`, null); 
}

}
