import { Injectable } from "@angular/core";
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from "@angular/router";
import { AuthService } from "./auth.service";
import { Observable } from "rxjs";



@Injectable()

export class RoutingGuard implements CanActivate{
    constructor(private authService:AuthService, private router:Router){}

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree | Observable<boolean | UrlTree> | Promise<boolean | UrlTree> {
        if(this.authService.isLoggedIn && ((route.url[0].path == 'login')||(route.url[0].path == 'signup')))
        {
            return false;
        }

        if(!this.authService.isLoggedIn && ((route.url[0].path == 'inbox')||route.url[0].path == 'changePassword'))
        {
            this.router.navigateByUrl('login');
            return false;
        }
        return true;
    }
}