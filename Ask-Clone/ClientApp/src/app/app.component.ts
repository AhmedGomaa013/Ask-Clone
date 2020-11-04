import { Component, OnInit } from '@angular/core';
import { AuthService } from './shared/auth.service';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  constructor(private authService: AuthService) { }
  ngOnInit(): void {
    const cookies = document.cookie;
    if (cookies === "UNL=LIT") {
      this.authService.isLoggedIn = true;
    }
  }
}
