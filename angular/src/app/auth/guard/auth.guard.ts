import { inject } from '@angular/core';
import { Router, ActivatedRouteSnapshot, RouterStateSnapshot, CanActivateFn } from '@angular/router';

export const authGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
    const router = inject(Router);

    if (localStorage.getItem('accessToken')) {
        return true;
    }
    // not logged in so redirect to welcome page with the return url
    router.navigate(['/welcome'], { queryParams: { returnUrl: state.url }});
    return false;
};