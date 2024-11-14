import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { NavigationEnd, Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent implements OnInit, OnDestroy {
  public isAuthenticated!: boolean;
  public dropdownOpen!:boolean;
  private router: Router = new Router;
  private routerSubscription!: Subscription;

  ngOnInit(): void {
    this.dropdownOpen = false;
    this.isAuthenticated = this.checkAuthentication();

    this.routerSubscription = this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.dropdownOpen = false;
        this.isAuthenticated = this.checkAuthentication();
      }
    });
  }

  toggleDropdown(event: Event): void {
    event.preventDefault();
    this.dropdownOpen = !this.dropdownOpen;
  }

  ngOnDestroy(): void {
    if (this.routerSubscription) {
      this.routerSubscription.unsubscribe();
    }
  }


  checkAuthentication(): boolean {
    return !!localStorage.getItem('accessToken');
  }

  logout(): void {
    localStorage.removeItem('accessToken');
    this.isAuthenticated = false;
    this.router.navigate(['/welcome']);
  }
}
