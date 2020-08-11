import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UserService } from '../user-service';
import { SearchResult } from '../search-result';
import { AuthService } from '../auth.service';

export interface DialogData {
  title: string,
  username: string
}
@Component({
  selector: 'app-follow',
  templateUrl: './follow.component.html',
  styleUrls: ['./follow.component.css']
})
export class FollowComponent implements OnInit {

  constructor(private dialogRef: MatDialogRef<FollowComponent>, private userService: UserService, private auth: AuthService,
    @Inject(MAT_DIALOG_DATA) public data: DialogData) { }

  users: SearchResult[] = [];
  showUsers: boolean = false;

  onFollow(user: SearchResult)
  {
    this.userService.follow(user.userName).subscribe({
      next: success => user.isFollowed = true,
      error: err => console.log(err)
    });
  }

  onUnfollow(user: SearchResult)
  {
    this.userService.unfollow(user.userName).subscribe({
      next: success => user.isFollowed = false,
      error: err => console.log(err)
    });
  }

  ngOnInit() {
    if (this.data.title == 'Followers')
    {
      this.userService.getFollowers(this.data.username).subscribe({
        next: (users: any) => {
          this.users = users;
          this.showUsers = true;
        },
        error: err => console.log(err)
      });
    }
    else if (this.data.title == 'Following')
    {
      this.userService.getFollowing(this.data.username).subscribe({
        next: (users: any) => {
          this.users = users;
          this.showUsers = true;
        },
        error: err => console.log(err)
      });
    }
  }
}
