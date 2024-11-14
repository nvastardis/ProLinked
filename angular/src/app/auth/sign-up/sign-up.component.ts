import { Component, inject } from '@angular/core';
import { FormGroup, Validators, ReactiveFormsModule, NonNullableFormBuilder, AbstractControl, ValidatorFn, ValidationErrors } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../openapi/api/auth.service';
import { RegisterRequest } from '../../../openapi/model/registerRequest';
import { CommonModule } from '@angular/common';
import { ErrorAlertComponent } from "../../components/error-alert/error-alert.component";
import { SuccessAlertComponent } from "../../components/success-alert/success-alert.component";
import { InputComponent } from "../../components/input/input.component";

@Component({
  selector: 'app-sign-up',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ErrorAlertComponent,
    SuccessAlertComponent,
    InputComponent
],
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.css']
})
export class SignUpComponent{
  successMessage: string | null = null;
  errorMessage: string | null = null;
  errors: string[] = [];
  signUpForm: FormGroup;
  authService = inject(AuthService);
  fb = inject(NonNullableFormBuilder);
  router = inject(Router);

  constructor() {
    this.signUpForm = this.fb.group({
      name: [null, [Validators.required]],
      surname: [null, [Validators.required]],
      userName: [null, [Validators.required]],
      email: [null, [Validators.required, Validators.email]],
      password: [null, [Validators.required, this.passwordStrengthValidator]],
      confirmPassword: [null, [Validators.required]],
      dateOfBirth: [null, [Validators.required]],
      phoneNumber: [null, [Validators.pattern('^[0-9]{10}$')]],
      summary: [null, [Validators.maxLength(150)]],
      jobTitle: [null, [Validators.maxLength(80)]],
      company: [null, [Validators.maxLength(80)]],
      city: [null, [Validators.maxLength(80)]],
    },
    { validators: [this.passwordMatchValidator] });
  }

  private passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const passwordControl = control.get('password');
    const confirmPasswordControl = control.get('confirmPassword');

    if (!passwordControl || !confirmPasswordControl) {
      return null;
    }

    const mismatch = passwordControl.value !== confirmPasswordControl.value;
    return mismatch ? { passwordMismatch: true } : null;
  }

  private passwordStrengthValidator(control: AbstractControl): ValidationErrors | null {
    if (!control) {
      return null;
    }

    const password = control.value;
    if(!password){
      return null;
    }
    const hasNumber = /\d/.test(password);
    const hasUpper = /[A-Z]/.test(password);
    const hasLower = /[a-z]/.test(password);
    const hasSpecial = /[!@#$%^&*(),.?":{}|<>]/.test(password);
    const hasMinLength = password.length >= 6;

    if (!hasNumber || !hasUpper || !hasLower || !hasSpecial || !hasMinLength) {
      return { passwordStrength: true };
    }
    return null;
  }

  onSubmit() {
    let signUpForm = this.signUpForm;
    this.successMessage = null;
    this.errorMessage = null;
    this.errors = [];

    this.signUpForm.markAllAsTouched();
    if (signUpForm.valid) {
      let errorAlert = document.getElementsByTagName('app-error-alert');
      if(errorAlert.length > 0){
        errorAlert[0].remove();
      }
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
            setTimeout(() => {
              this.router.navigate(['/sign-in']);
            }, 1000);
          },
          error: error => {
            this.errors = Object.keys(error.error.errors).map(function (key) { return error.error.errors[key]+'\r\n'; });
            this.errorMessage = 'Registration failed';
          }
      });
    }
  }

  get nameValidationErrors(){
    let errorsToMessages: {key: string, value: string}[] = [];
    if(this.signUpForm.hasError('required', 'name') && this.signUpForm.controls['name'].touched){
      errorsToMessages.push({key:'required',value:'Name is required'});
    }
    return errorsToMessages;
  }

  get surnameValidationErrors(){
    let errorsToMessages: {key: string, value: string}[] = [];
    if(this.signUpForm.hasError('required', 'surname') && this.signUpForm.controls['surname'].touched){
      errorsToMessages.push({key:'required',value:'Surname is required'});
    }
    return errorsToMessages;
  }

  get userNameValidationErrors(){
    let errorsToMessages: {key: string, value: string}[] = [];
    if(this.signUpForm.hasError('required', 'userName') && this.signUpForm.controls['userName'].touched){
      errorsToMessages.push({key:'required',value:'Username is required'});
    }
    return errorsToMessages;
  }

  get emailValidationErrors(){
    let errorsToMessages: {key: string, value: string}[] = [];
    if(this.signUpForm.hasError('required', 'email') && this.signUpForm.controls['email'].touched){
      errorsToMessages.push({key:'required',value:'Email is required'});
    }
    if(this.signUpForm.hasError('email', 'email') && this.signUpForm.controls['email'].touched){
      errorsToMessages.push({key:'email',value:'Email is not valid'});
    }
    return errorsToMessages;
  }

  get passwordValidationErrors(){
    let errorsToMessages: {key: string, value: string}[] = [];
    if(this.signUpForm.hasError('required', 'password') && this.signUpForm.controls['password'].touched){
      errorsToMessages.push({key:'required',value:'Password is required'});
    }
    if(this.signUpForm.hasError('passwordStrength', 'password') && this.signUpForm.controls['password'].touched){
      if(!this.signUpForm.controls['password'].value?.match('^(?=.*[A-Z])'))
      {
        errorsToMessages.push({key:'passwordStrength-uppercase',value:'Password requires at least one Uppercase Character'});
      }
      if(!this.signUpForm.controls['password'].value?.match('^(?=.*[a-z])'))
      {
        errorsToMessages.push({key:'passwordStrength-lowercase',value:'Password requires at least one Lowercase Character'});
      }
      if(!this.signUpForm.controls['password'].value?.match('^(?=.*[0-9])'))
      {
        errorsToMessages.push({key:'passwordStrength-number',value:'Password requires at least one Number'});
      }
      if(!this.signUpForm.controls['password'].value?.match('^(?=.*[!@#$%^&*])'))
      {
        errorsToMessages.push({key:'passwordStrength-special',value:'Password requires at least 1 special character from: \'!\', \'@\', \'#\', \'$\',\'%\', \'&\' CharacterData.'});
      }
      if(!this.signUpForm.controls['password'].value?.match('.{6,}'))
      {
        errorsToMessages.push({key:'passwordStrength-length',value:'Passwrod must be minimum 6 CharactersCharacterData.'});
      }
    }
    return errorsToMessages;
  }

  get confirmPasswordValidationErrors(){
    let errorsToMessages: {key: string, value: string}[] = [];
    if(this.signUpForm.hasError('required', 'confirmPassword') && this.signUpForm.controls['confirmPassword'].touched){
      errorsToMessages.push({key:'required',value:'Confirm Password is required'});
    }
    if(this.signUpForm.hasError('passwordMismatch') && !this.signUpForm.hasError('required', 'confirmPassword') && this.signUpForm.controls['confirmPassword'].touched){
      errorsToMessages.push({key:'passwordMismatch',value:'Passwords do not match'});
    }
    return errorsToMessages;
  }

  get dateOfBirthValidationErrors(){
    let errorsToMessages: {key: string, value: string}[] = [];
    if(this.signUpForm.hasError('required', 'dateOfBirth') && this.signUpForm.controls['dateOfBirth'].touched){
      errorsToMessages.push({key:'required',value:'Date of Birth is required'});
    }
    return errorsToMessages;
  }

  get phoneNumberValidationErrors(){
    let errorsToMessages: {key: string, value: string}[] = [];
    if(this.signUpForm.hasError('pattern', 'phoneNumber') && this.signUpForm.controls['phoneNumber'].touched){
      errorsToMessages.push({key:'pattern',value:'Phone Number is not valid'});
    }
    return errorsToMessages;
  }

 
  get summaryValidationErrors(){
    let errorsToMessages: {key: string, value: string}[] = [];
    if(this.signUpForm.hasError('maxlength', 'summary') && this.signUpForm.controls['summary'].touched){
      errorsToMessages.push({key:'maxlength',value:'Summary is too long'});
    }
    return errorsToMessages;
  }

  
  get jobTitleValidationErrors(){
    let errorsToMessages: {key: string, value: string}[] = [];
    if(this.signUpForm.hasError('maxlength', 'jobTitle') && this.signUpForm.controls['jobTitle'].touched){
      errorsToMessages.push({key:'maxlength',value:'Job Title is too long'});
    }
    return errorsToMessages;
  }

  get companyValidationErrors(){
    let errorsToMessages: {key: string, value: string}[] = [];
    if(this.signUpForm.hasError('maxlength', 'company') && this.signUpForm.controls['company'].touched){
      errorsToMessages.push({key:'maxlength',value:'Company is too long'});
    }
    return errorsToMessages;
  }

  get cityValidationErrors(){
    let errorsToMessages: {key: string, value: string}[] = [];
    if(this.signUpForm.hasError('maxlength', 'city') && this.signUpForm.controls['city'].touched){
      errorsToMessages.push({key:'maxlength',value:'City is too long'});
    }
    return errorsToMessages;
  }
}
