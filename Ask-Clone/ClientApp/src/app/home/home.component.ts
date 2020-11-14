import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { AuthService } from '../shared/auth.service';
import { Question } from '../shared/question';
import { UserService } from '../shared/user-service';
import { DataService } from '../shared/data.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(
    private title: Title,
    private authService: AuthService,
    private userService: UserService,
    private dataService: DataService) { }

  questions: Question[] = [];
  showComponent: boolean = false;

  ngOnInit() 
  {
    this.title.setTitle("Ask-Clone");
    if (this.authService.isLoggedIn) {
      this.userService.getHomeQuestions()
        .subscribe({
        next: questions => {
          this.questions = questions;
          this.dataService.sort(this.questions);
          this.showComponent = true;
        }
      });
    }
  }

}
