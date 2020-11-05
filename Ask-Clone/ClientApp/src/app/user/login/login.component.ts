import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { LoginUser } from 'src/app/shared/login-user';
import { UserService } from 'src/app/shared/user-service';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'src/app/shared/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  constructor(private title:Title, private userService:UserService, private router:Router,
    private toastr:ToastrService, private auth:AuthService) { }

  errorMessage:string = ""
  connectionMade: boolean = false;
  user:LoginUser={
    password:"",
    username:""
  };

  onLogin()
  {
    if (this.user.username.length < 5 || this.user.username.length > 25)
    {
      this.errorMessage = "Username must be at least 5 characters and at most 25 characters";
    }
    else if((this.user.username)&&(this.user.password))
    {
    this.connectionMade = true;
    this.userService.login(this.user).subscribe(
      (res:any)=>{
        
        this.auth.username = this.user.username;
        this.auth.validateLogin();
        this.toastr.success("Welcome, " + this.auth.username);
        this.router.navigate(['inbox', this.auth.username]);
      },
      err=>{
        this.connectionMade = false;
        this.errorMessage = '';
        if(err.status == 400) 
        {
          this.errorMessage = 'Incorrect Username or Password';
          this.toastr.error('Incorrect Username or Password', 'Authentication Failed');
        }
        else 
        {
          console.log(err);
        }
      }
    );
  }
  else
  {
    this.errorMessage = 'Username or Password can\'t be empty';
  }
  }


  ngOnInit() {
    
    this.title.setTitle("Log In to Notes");

  }

}
