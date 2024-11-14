import { Component, inject } from '@angular/core';
import { FormGroup, ReactiveFormsModule, NonNullableFormBuilder, Validators } from '@angular/forms';
import { AuthService } from '../../../openapi/api/auth.service';
import { LoginRequest } from '../../../openapi';
import { Router } from '@angular/router';
import { InputComponent } from "../../components/input/input.component";
import { ErrorAlertComponent } from "../../components/error-alert/error-alert.component";

@Component({
  selector: 'app-sign-in',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    InputComponent,
    ErrorAlertComponent
],
  templateUrl: './sign-in.component.html',
  styleUrl: './sign-in.component.css'
})
export class SignInComponent{
  errorMessage: string | null = null;
  errors: string[] = [];
  signInForm: FormGroup;
  authService = inject(AuthService);
  fb = inject(NonNullableFormBuilder);
  router = inject(Router);

  constructor() {
    if(this.checkAuthentication()){
      this.router.navigate(['/home']);
    }

    this.signInForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  
  onSubmit(){
    let signInForm = this.signInForm;
    this.errorMessage = null;
    this.errors = [];

    if (signInForm.valid) {
      let errorAlert = document.getElementsByTagName('app-error-alert');
      if(errorAlert.length > 0){
        errorAlert[0].remove();
      }
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
          this.errorMessage = "Login Failed"
          this.errors = [error.error.detail];
        }
      });
    }
  }

  checkAuthentication(): boolean {
    return !!localStorage.getItem('accessToken');
  }

  get usernameValidationErrors(){
    let errorsToMessages: {key: string, value: string}[] = [];
    if(this.signInForm.hasError('required', 'username') && this.signInForm.controls['username'].touched){
      errorsToMessages.push({key:'required',value:'Username is required'});
    }
    return errorsToMessages;
  }

  get passwordValidationErrors(){
    let errorsToMessages: {key: string, value: string}[] = [];
    if(this.signInForm.hasError('required', 'password') && this.signInForm.controls['password'].touched){
      errorsToMessages.push({key:'required',value:'Password is required'});
    }
    return errorsToMessages;
  }
}
