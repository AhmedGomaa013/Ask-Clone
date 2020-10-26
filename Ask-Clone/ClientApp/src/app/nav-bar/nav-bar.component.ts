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

  myFunction() {
    document.getElementById("myDropdown").classList.toggle("show");
  }

  onKeyPressed(event)
  {
    if(event.keyCode == 13)
    {
      this.router.navigate(['search'], { queryParams: { username: this.searchValue } });
    }
  }
  onLogout(){
    this.auth.token = '';
    this.auth.username = '';
    this.searchValue = '';
    this.auth.clearCreds();
    this.router.navigateByUrl('login');
  }
}
