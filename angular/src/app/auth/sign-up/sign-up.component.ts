import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { FormGroup, Validators, ReactiveFormsModule, NonNullableFormBuilder, AbstractControl, ValidatorFn, ValidationErrors } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../openapi/api/auth.service';
import { RegisterRequest } from '../../../openapi/model/registerRequest';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-sign-up',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
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
    this.signUpForm.markAllAsTouched();
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
            this.errors = Object.keys(error.error.errors).map(function (key) { return error.error.errors[key]+'\r\n'; });
            this.errorMessage = 'Registration failed';
            this.successMessage = null;
          }
      });
    }
  }

  get nameRequiredError(){
    return this.signUpForm.hasError('required', 'name') && this.signUpForm.controls['name'].touched;
  }

  get surnameRequiredError(){
    return this.signUpForm.hasError('required', 'surname') && this.signUpForm.controls['surname'].touched;
  }

  get userNameRequiredError(){
    return this.signUpForm.hasError('required', 'userName') && this.signUpForm.controls['userName'].touched;
  }

  get emailRequiredError(){
    return this.signUpForm.hasError('required', 'email') && this.signUpForm.controls['email'].touched;
  }

  get emailFormatError(){
    return this.signUpForm.hasError('email', 'email') && this.signUpForm.controls['email'].touched;
  }

  get passwordRequiredError(){
    return this.signUpForm.hasError('required', 'password') && this.signUpForm.controls['password'].touched;
  }

  get passwordStrengthError(){
    return this.signUpForm.hasError('passwordStrength', 'password') && this.signUpForm.controls['password'].touched;
  }

  get confirmPasswordRequiredError(){
    return this.signUpForm.hasError('required', 'confirmPassword') && this.signUpForm.controls['confirmPassword'].touched;
  }

  get passwordMismatchError(){
    return this.signUpForm.hasError('passwordMismatch') && !this.signUpForm.hasError('required', 'confirmPassword') && this.signUpForm.controls['confirmPassword'].touched;
  }

  get dateOfBirthRequiredError(){
    return this.signUpForm.hasError('required', 'dateOfBirth') && this.signUpForm.controls['dateOfBirth'].touched;
  }

  get phoneNumberPatternError(){
    return this.signUpForm.hasError('pattern', 'phoneNumber') && this.signUpForm.controls['phoneNumber'].touched;
  }

  get summaryMaxLengthError(){
    return this.signUpForm.hasError('maxlength', 'summary') && this.signUpForm.controls['summary'].touched;
  }

  get jobTitleMaxLengthError(){
    return this.signUpForm.hasError('maxlength', 'jobTitle') && this.signUpForm.controls['jobTitle'].touched;
  }

  get companyMaxLengthError(){
    return this.signUpForm.hasError('maxlength', 'company') && this.signUpForm.controls['company'].touched;
  }

  get cityMaxLengthError(){
    return this.signUpForm.hasError('maxlength', 'city') && this.signUpForm.controls['city'].touched;
  }
}
