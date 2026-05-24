import { Component, OnInit } from '@angular/core';
import { AuthService } from './services/auth.service';
import { UserDetail } from './models/user.model';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'UserRegistrationApp';
  editData: UserDetail | null = null;
  isAuthenticated = false;

  constructor(private authService: AuthService) { }

  ngOnInit(): void {
    this.isAuthenticated = this.authService.isAuthenticated();
  }

  onEditData(data: UserDetail): void {
    this.editData = data;
  }

  onDataUpdated(): void {
    this.editData = null;
  }

  onLoginSuccess(): void {
    this.isAuthenticated = true;
  }

  logout(): void {
    this.authService.logout();
    this.isAuthenticated = false;
    this.editData = null;
  }
}
