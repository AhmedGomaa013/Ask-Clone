import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor() { }

  isLoggedIn:boolean = false;

  getUsernameFromToken()
  {
    if (this.token) {
      let payload = this.token.split('.')[1];
      if (payload) {
        let username = JSON.parse(atob(payload))["UserName"];
        if (username) {
          this.username = username;
        }
      }
    }
}


  get username(): string {
    return localStorage.getItem('username');
  }
  set username(value: string) {
    localStorage.setItem('username', value);
  }

  get token():string{
    return localStorage.getItem('token');
  }
  set token(value:string){
    localStorage.setItem('token', value);
  }

  clearCreds()
  {
    localStorage.clear();
    this.token='';
    this.username = '';
    this.isLoggedIn = false;
  }

  isLoggedInFunc(){
    if(this.token)
    {
      let payload = this.token.split('.')[1];
      if(payload)
      {
        let expire = JSON.parse(atob(payload))["exp"];
        if(expire)
        {     
          if(expire>(Date.now())/1000) 
          {
            this.isLoggedIn = true;
            return;
          }
        }
      }
    }
    this.clearCreds();
  }
}
