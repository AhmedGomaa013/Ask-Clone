import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../shared/user-service';
import { SearchResult } from '../shared/search-result';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-search-result',
  templateUrl: './search-result.component.html',
  styleUrls: ['./search-result.component.css']
})
export class SearchResultComponent {


  constructor(private route: ActivatedRoute, private router: Router, private userService: UserService, private title: Title) {
    route.params.subscribe(para => this.onInit());
  }
  searchValue:string='';
  searchResult: SearchResult[] = [];
  showComponent: boolean = false;

  onFollow(result: SearchResult)
  {
    this.userService.follow(result.userName).subscribe({
      next: success => {
        result.isFollowed = true;
      },
      error: err => console.log(err)
    });
  }

  onUnfollow(result: SearchResult)
  {
    this.userService.unfollow(result.userName).subscribe({
      next: success => {
        result.isFollowed = false;
      },
      error: err => console.log(err)
    });
  }

  onInit() {
    this.title.setTitle("Ask-Clone");
    this.route.queryParams.subscribe(params => {
      this.showComponent = false;
      this.searchValue = params['username'];

      this.userService.search(this.searchValue).subscribe({
        next: results => {
          this.searchResult = results;
          this.showComponent = true;
        },
        error: err => console.log(err)
      });
    });
  }
}
