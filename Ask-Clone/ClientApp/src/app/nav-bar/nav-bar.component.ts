import { Component } from '@angular/core';
import { AuthService } from '../shared/auth.service';
import { Router } from '@angular/router';
import { UserService } from '../shared/user-service';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.css']
})
export class NavBarComponent{

  constructor(private auth:AuthService, private router:Router, private userService:UserService) { }

  searchValue:string = '';

  onTest(event)
  {
    if(event.keyCode == 13)
    {
      this.userService.search(this.searchValue).subscribe({
        next: results => {
          this.userService.searchResult = results;
          this.router.navigate(['search'],{queryParams:{username:this.searchValue}});
        },
        error: err => console.log(err)
      });
    }
  }
  onLogout(){
    this.auth.token = '';
    this.auth.username = '';
    this.auth.clearCreds();
    this.router.navigateByUrl('login');
  }
}
