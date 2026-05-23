import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  loginForm:FormGroup = new FormGroup({});

  constructor(  private fb: FormBuilder,
  private authService: AuthService,
  private router: Router)
  {

  }
  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email: [''],
      password: ['']
    });
  }
  login() {

  if(this.loginForm.invalid){
    return;
  }

  this.authService.login(this.loginForm.value)
    .subscribe({
      next: (res: any) => {

        this.authService.setToken(res.token);

        alert('Login Successful');

        this.router.navigate(['/users']);
      },

      error: () => {
        alert('Invalid Credentials');
      }
    });
}
}
