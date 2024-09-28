import { Injectable } from '@angular/core';
import { Router, CanMatchFn, ActivatedRouteSnapshot, RouterStateSnapshot, CanActivateFn, CanActivate } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivateFn {

    constructor(private router: Router) { }

    canActivateFn(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
        if (localStorage.getItem('token')) {
            // logged in so return true
            return true;
        }
        // not logged in so redirect to login page with the return url
        this.router.navigate(['/login'], { queryParams: { returnUrl: state.url }});
        return false;
    }
}