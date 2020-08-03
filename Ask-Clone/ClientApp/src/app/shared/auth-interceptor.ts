import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from "@angular/common/http";
import { Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { AuthService } from "./auth.service";

@Injectable()

export class AuthInterceptor implements HttpInterceptor {
    constructor(private router:Router, private auth:AuthService){}
    
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        if(this.auth.isLoggedIn)
        {
            const clonereq = req.clone({
                headers: req.headers.set('Authorization', 'Bearer ' + this.auth.token)
            });

            return next.handle(clonereq).pipe(
                tap(
                    success =>{},
                    err =>{
                        if (err.status == 401)
                        {
                            this.auth.clearCreds();
                            this.router.navigateByUrl('login');
                        }
                    }
                )
            );
        }
        else
        {
            return next.handle(req.clone());
        }
    }

}
