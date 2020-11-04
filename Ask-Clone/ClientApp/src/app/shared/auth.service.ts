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
    localStorage.setItem('username', value);
  }

  clearCreds()
  {
    localStorage.clear();
    this.username = '';
    this.isLoggedIn = false;
  }
}
