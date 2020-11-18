import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Passwords } from 'src/app/shared/passwordsChange';
import { UserService } from 'src/app/shared/user-service';
import { AuthService } from '../../shared/auth.service';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent implements OnInit {

  constructor(private title: Title, private userService: UserService, private toast: ToastrService,
    private router: Router, private authService: AuthService) { }

  errorMessage:string = '';
  passwords:Passwords ={
    currentPassword:'',
    newPassword:'',
    confirmNewPassword:''
  };
  connectionMade:boolean=false;

  onConfirm(){
    if((this.passwords.newPassword != this.passwords.confirmNewPassword) && (this.passwords.newPassword != '') && (this.passwords.confirmNewPassword != ''))
    {
      this.errorMessage = 'Password and Confirm Password doesn\'t match';
    }
    else if((this.passwords.currentPassword == '') || (this.passwords.newPassword == '') || (this.passwords.confirmNewPassword == ''))
    {
      this.errorMessage = "Make sure all the fields aren\'t empty";
    }
    else
    {
      this.connectionMade = true;
      this.errorMessage = '';
      this.userService.changePassword(this.passwords).subscribe(
        {
          next: (res:any)=>{
            if(res.succeeded){
              this.errorMessage = "";
              this.toast.success('Password changed Successfully', 'Successful Operation');
              this.authService.clearCreds();
              this.router.navigateByUrl('login');
              this.toast.success('Log in again with the new password');
            }
            else{
              this.errorMessage = "";
              this.connectionMade = false;

              res.errors.forEach(element => {
                this.toast.error(element.description, 'Registration failed.');
                  this.errorMessage += element.description;
                  this.errorMessage += '\n';
              });
            }
          },
          error: (error)=>{
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
        }
      );
    }
  }

  ngOnInit() {
    this.title.setTitle('Change Password');
  }

}
