import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor() { }

  isLoggedIn:boolean = false;

  get username(): string {
    return localStorage.getItem('username');
  }
  set username(value: string) {
    if (value === '')
      localStorage.removeItem("username");
    else
      localStorage.setItem('username', value);
  }

  clearCreds()
  {
    this.username = '';
    this.isLoggedIn = false;
  }

  validateLogin() {
    const cookies = document.cookie;
    if (cookies === "UNL=LIT") {
      this.isLoggedIn = true;
    }
  }

}
