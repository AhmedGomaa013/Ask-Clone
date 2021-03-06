import { Component } from '@angular/core';
import { AuthService } from '../shared/auth.service';
import { Router } from '@angular/router';
import { UserService } from '../shared/user-service';

@Component({
  selector: 'app-nav-bar',
  host:{
    '(document:click)':'onClick($event)'
  },
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.css']
})
export class NavBarComponent{

  constructor(
    private auth:AuthService,
    private router:Router, 
    private userService:UserService) { }

  searchValue:string = '';

  onToggleDropDownMenu() {
    document.getElementById("Dropdown").classList.toggle("show");
  }

  onClick(event)
  {
    if(!event.target.matches(".fa-cog"))
    {
      document.getElementById("Dropdown").classList.remove("show");
    }
  }
  onKeyPressed(event)
  {
    if(event.keyCode == 13)
    {
      this.router.navigate(['search'], { queryParams: { username: this.searchValue } });
    }
  }

  onLogout() {

    this.userService.logout().subscribe(
      {
        next: success => {
          this.searchValue = '';
          this.auth.clearCreds();
          this.router.navigateByUrl('login');
        },
        error: err => {
          console.log(err);
        }
      });
  }
}
