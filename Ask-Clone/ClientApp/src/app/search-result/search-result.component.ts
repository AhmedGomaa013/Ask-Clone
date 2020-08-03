import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../shared/user-service';
import { SearchResult } from '../shared/search-result';

@Component({
  selector: 'app-search-result',
  templateUrl: './search-result.component.html',
  styleUrls: ['./search-result.component.css']
})
export class SearchResultComponent implements OnInit {

  
  constructor(private route: ActivatedRoute, private router: Router, private userService: UserService) {
    route.params.subscribe(para => this.ngOnInit());
  }
  searchValue:string='';
  searchResult:SearchResult[]=[];
  ngOnInit() {
    this.route.queryParams.subscribe(params =>{
      this.searchValue = params['username'];
      this.searchResult = this.userService.searchResult;
    });

  }
}
