import { Component } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../openapi/api/auth.service'; // Adjust the path as necessary
import { RegisterRequest } from '../../../openapi/model/registerRequest'; // Adjust the path as necessary

@Component({
  selector: 'app-sign-up',
  standalone: true,
  imports: [
    FormsModule
  ],
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.css']
})
export class SignUpComponent {
  successMessage: string | null = null;
  errorMessage: string | null = null;

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit(signUpForm: NgForm) {
    if (signUpForm.valid) {
      const body: RegisterRequest = {
        name: signUpForm.value.name,
        surname: signUpForm.value.surname,
        userName: signUpForm.value.userName,
        email: signUpForm.value.email,
        password: signUpForm.value.password,
        dateOfBirth: new Date(signUpForm.value.dateOfBirth),
        phoneNumber: signUpForm.value.phoneNumber,
        summary: signUpForm.value.summary,
        jobTitle: signUpForm.value.jobTitle,
        company: signUpForm.value.company,
        city: signUpForm.value.city
      };
      this.authService.register(body).subscribe({
          next: response => {
            this.successMessage = 'Registration successful! Redirecting to login...';
            this.errorMessage = null; 
            setTimeout(() => {
              this.router.navigate(['/sign-in']);
            }, 3000);
          },
          error: error => {
            var errors = Object.keys(error.error.errors).map(function (key) { return error.error.errors[key]+'\r\n'; });
            this.errorMessage = 'Registration failed: ' + errors.join('-');
            this.successMessage = null;
          }
      });
    }
  }
}