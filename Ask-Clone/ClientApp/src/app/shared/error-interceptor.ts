import { Injectable } from "@angular/core";
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse, HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { catchError, switchMap, finalize } from "rxjs/operators";
import { AuthService } from "./auth.service";
import { UserService } from "./user-service";
import { Router } from "@angular/router";

@Injectable()

export class ErrorInterceptor implements HttpInterceptor {
  constructor(
    private auth: AuthService,
    private userService: UserService,
    private router: Router) { }

  isRefreshingToken: boolean = true;
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (this.auth.isLoggedIn) {

      return next.handle(req).pipe(
        catchError(err => {
          if (err instanceof HttpErrorResponse && err.status === 401 && this.isRefreshingToken) {
            this.isRefreshingToken = false;
            return this.handle401Error(req, next);
          }
          else {
            throw err;
          }
        }
        ));
    }
    else {
      return next.handle(req);
    }
  }

  handle401Error(request: HttpRequest<any>, next: HttpHandler) {
    return this.userService.refreshToken(this.auth.username).pipe(
      catchError(err => {
        this.auth.clearCreds();
        this.router.navigateByUrl('login');
        return new Observable();
      }),
      switchMap(response => {
        return next.handle(request);
      }
      ),
      finalize(() => {
        this.isRefreshingToken = true;
      }));
  }
}
