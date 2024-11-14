import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-welcome',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './welcome.component.html',
  styleUrl: './welcome.component.css'
})
export class WelcomeComponent implements OnInit{
  private router: Router = new Router;
  
  ngOnInit(): void {
    if(this.checkAuthentication()){
      this.router.navigate(['/home']);
    }
  }
  
  checkAuthentication(): boolean {
    return !!localStorage.getItem('accessToken');
  }
}
