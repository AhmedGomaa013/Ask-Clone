import { Component, OnInit } from '@angular/core';
import { RegisterUser } from 'src/app/shared/register-user';
import { Title } from '@angular/platform-browser';
import { UserService } from 'src/app/shared/user-service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/shared/auth.service';



@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css']
})
export class SignupComponent implements OnInit {

  constructor(private title:Title,private userService:UserService, private auth:AuthService,
    private toastrService:ToastrService, private router:Router) { }

  errorMessage:string = '';
  connectionMade:boolean = false;

  user:RegisterUser={
    firstName:'', lastName:'',username:'',email:'',password:'',confirmPassword:''
  };

  onCreate()
  {
    if ((this.user.password != this.user.confirmPassword) && (this.user.password != '') && (this.user.confirmPassword != '')) {
      this.errorMessage = 'Password and Confirm Password doesn\'t match';
    }
    else if (this.user.username.length < 5 || this.user.username.length > 25)
    {
      this.errorMessage = "Username must be at least 5 characters and at most 25 characters";
    }
    else if((this.user.firstName == '') && (this.user.lastName == '') && (this.user.username == '') && 
    (this.user.password == '') && (this.user.confirmPassword == ''))
    {
      this.errorMessage = 'Make sure all the fields aren\'t empty';
    }
    else{
      this.connectionMade = true;
      this.errorMessage = '';
      this.userService.signup(this.user).subscribe(
        (response: any) => {
          if (response.succeeded) {
            this.toastrService.success("New User Created", "Registration Successful");
            this.errorMessage = "";
            this.router.navigateByUrl('login');
          }
          else {
            this.errorMessage = "";
            this.connectionMade = false;

            response.errors.forEach(element => {
              switch (element.code) {
                case 'DuplicateUserName':
                  this.toastrService.error('Username is already taken', 'Registration failed.');
                  this.errorMessage = "Username is already taken";
                  break;

                default:
                  this.toastrService.error(element.description, 'Registration failed.');
                  this.errorMessage += element.description;
                  this.errorMessage += '\n';
                  break;
              }
            });
          }
        },
        error => {
          this.connectionMade = false;
          if (error.status == 400)
          {
            this.errorMessage = error.message;
          }
          else
          {
            console.log(error);
          }
        }
      );
    }
  }

  ngOnInit() 
  {
    if(this.auth.isLoggedIn)
    
    this.title.setTitle("Create Ask-Clone account");
  }

}
