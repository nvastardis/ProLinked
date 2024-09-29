import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { AuthService } from '../../../openapi/api/auth.service';
import { LoginRequest } from '../../../openapi';
import { Router } from '@angular/router';

@Component({
  selector: 'app-sign-in',
  standalone: true,
  imports: [
    FormsModule
  ],
  templateUrl: './sign-in.component.html',
  styleUrl: './sign-in.component.css'
})
export class SignInComponent implements OnInit {
  public isAuthenticated!: boolean;
  constructor(private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
    if(this.checkAuthentication()){
      this.router.navigate(['/home']);
    }
  }

  
  onSubmit(signInForm: NgForm) {
    if (signInForm.valid) {
      var body:LoginRequest = {
        username: signInForm.value.username,
        password: signInForm.value.password
      } 
      this.authService.login(body).subscribe({
        next: response => {
          const accessToken = response.accessToken;
          const refreshToken = response.refreshToken;
          const expiration = response.expiresIn;

          localStorage.setItem('accessToken', accessToken);
          localStorage.setItem('refreshToken', refreshToken);
          localStorage.setItem('expiration', expiration);
          this.router.navigate(['/home']);
        },
        error: error => {
          console.error('Login failed:', error);
        }
      });
    }
  }

  checkAuthentication(): boolean {
    return !!localStorage.getItem('accessToken');
  }
}
