import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../../../openapi/api/auth.service';
import { LoginRequest } from '../../../../openapi';

@Component({
  selector: 'app-signin',
  standalone: true,
  imports: [],
  templateUrl: './signin.component.html',
  styleUrl: './signin.component.css'
})
export class SigninComponent implements OnInit {
  model: any = {};
  loading = false;
  returnUrl: string = '/';

  constructor(
      private route: ActivatedRoute,
      private router: Router,
      private authenticationService: AuthService
  ) { }

  ngOnInit(): void {
      this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }

  login(): void {
    let body:LoginRequest = {
      username: this.model.username,
      password: this.model.password
    }

      this.loading = true;
      this.authenticationService.login(body)
          .subscribe(
              response => {
                localStorage.setItem('token', <string>response.headers.get('Authorization'));
                this.router.navigate([this.returnUrl]); }
          );
  }
}