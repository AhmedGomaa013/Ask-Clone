import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';

import { ToastrService } from 'ngx-toastr';

import { DataService } from 'src/app/shared/data.service';
import { Question } from 'src/app/shared/question';
import { AuthService } from '../../shared/auth.service';
import { OpenDialogComponent } from 'src/app/shared/open-dialog/open-dialog.component';
import { UserService } from 'src/app/shared/user-service';
import { Title } from '@angular/platform-browser';
import { FollowComponent } from '../../shared/follow/follow.component';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent{
  
  constructor(private dataService: DataService, private router: Router, private authService: AuthService,
    private toastrService: ToastrService, private dialog: MatDialog, private route: ActivatedRoute,
    private userService: UserService, private title: Title)
    {
      route.params.subscribe(params => {
        this.onCreate();
      });
    }
  
  showComponent:boolean = false;
  showError:boolean = false;
  username:string = '';
  question:string='';
  questions:Question[]=[];
  anon:boolean=false;
  isFollowed:boolean = false;
  followersNumber:number=0;
  followingNumber:number=0;

  onFollowing()
  {
    const dialogRef = this.dialog.open(FollowComponent, {
      data: { title: "Following", username: this.username }, panelClass: 'dialog', width: '72%'
    });
    
    dialogRef.afterClosed().subscribe(res=>{
      this.onCreate();
    })
  }

  onFollowers()
  {
    const dialogRef = this.dialog.open(FollowComponent, {
      data: { title: "Followers", username: this.username }, panelClass: 'dialog', width: '72%'
    });

    dialogRef.afterClosed().subscribe(res=>{
      this.onCreate();
    });
  }

  onFollow()
  {
    this.userService.follow(this.username).subscribe({
      next: success => {
        this.userService.followingFollowersNumber(this.username).subscribe({
          next: (numbers: any) => {
            this.followingNumber = numbers.following;
            this.followersNumber = numbers.followers;
          },
          error: err => console.log(err)
        });
        this.isFollowed = true
      },
          error: err=> console.log(err)
      });
  }

  onUnfollow()
  {
    this.userService.unfollow(this.username).subscribe({
      next: succuss => {
        this.userService.followingFollowersNumber(this.username).subscribe({
          next: (numbers: any) => {
            this.followingNumber = numbers.following;
            this.followersNumber = numbers.followers;
          },
          error: err => console.log(err)
        });
        this.isFollowed = false
      },
          error: err => console.log(err)
      });
  }
  
  onAsk()
  {
    let tempQuestion: Question = {
      questionId: 0,
      answer: null,
      isAnswered: false,
      questionFrom:null,
      question: this.question,
      time: new Date(),
      questionTo: this.username
    };
    
    if (this.authService.isLoggedIn && !this.anon) tempQuestion.questionFrom = this.authService.username;
    if((this.question != null)&&(this.question.length <= 300))
    {
    this.dataService.post(this.username, tempQuestion).subscribe(
      {
        next: success => {
          this.toastrService.success("Question sent Sucessfully");
          this.question = '';
        },
        error: err => {
          console.log(err);
          if (err.status == 400)
          {
            this.toastrService.error("Send it Again!!", "Operation Failed!");
          }
        }
      });}
      else
      {
        this.toastrService.error( "Your question can't exceed 300 characters!");
      }
  }

  onConfirmDeletion(question:Question)
  {
    
    const openedDialog = this.dialog.open(OpenDialogComponent,{
      data:  {title:"Warning",content:'Are you sure you want to permanently delete this question?',delete:true,answer:''}
    });
    openedDialog.afterClosed().subscribe(result => {
      if(result == 1)
      {
        this.onDeleteQuestion(question);
      }
    })

  }

  onDeleteQuestion(question:Question)
  {
    this.dataService.delete(question.questionId).subscribe({
      next: success=>{
        let index:number = this.questions.indexOf(question,0);
        this.questions.splice(index,1);
        this.toastrService.success("Question Deleted Successfully");
      },
      error: err =>{
        this.toastrService.error("Couldn't delete this question","Operation Failed");
      }
    });
  }

  onSort()
  {
    this.dataService.sort(this.questions);
  }
  
  onCreate()
  {
    this.username = this.router.url.slice(6);
    if (!this.username.length) this.router.navigateByUrl('/');

    this.title.setTitle(this.username);

    if (this.authService.isLoggedIn && this.username != this.authService.username)
    {
      this.userService.isFollowed(this.username).subscribe({
        next: success => {
          this.isFollowed = success;
        },
        error: err => {
          console.log(err);
        }
      });
    }

    this.userService.followingFollowersNumber(this.username).subscribe({
      next:(numbers:any)=>{
        this.followingNumber = numbers.following;
        this.followersNumber = numbers.followers;
      },
      error:err=>console.log(err)
    });

    this.dataService.getAnsweredQuestions(this.username).subscribe({
      next: questions => {
        this.showComponent = true;
        this.questions = questions;
        this.onSort();
      },
      error: err => {
        if (err.status == 404) this.showError = true;
        else console.log(err);
      }
    });
  }

}
