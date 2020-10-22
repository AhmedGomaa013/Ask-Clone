import { HttpClient } from "@angular/common/http";
import { LoginUser } from "./login-user";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { RegisterUser } from "./register-user";
import { SearchResult } from "./search-result";
import { Observable } from "rxjs";
import { Question } from "./question";

@Injectable()
export class UserService {
  constructor(private http: HttpClient, private router: Router) { }

  searchResult: SearchResult[] = [];

  private readonly baseUrl: string = "api/User/";

  login(user: LoginUser) {
    return this.http.post(this.baseUrl + 'login', user);
  }

  signup(user: RegisterUser) {
    return this.http.post(this.baseUrl + 'Register', user);
  }

  search(searchValue: string): Observable<SearchResult[]> {
    return this.http.get<SearchResult[]>(this.baseUrl + 'search', { params: { username: searchValue } });
  }

  follow(username: string) {
    return this.http.get(this.baseUrl + 'follow/' + username);
  }

  unfollow(username: string) {
    return this.http.get(this.baseUrl + 'unfollow/' + username);
  }

  isFollowed(username: string): Observable<boolean> {
    return this.http.get<boolean>(this.baseUrl + 'isfollowing/' + username);
  }

  getFollowers(username: string) {
    return this.http.get(this.baseUrl + 'followers/' + username);
  }

  getFollowing(username: string) {
    return this.http.get(this.baseUrl + 'following/' + username);
  }

  followingFollowersNumber(username: string) {
    return this.http.get(this.baseUrl + 'FollowingFollowersNumber/' + username);
  }

  getHomeQuestions(): Observable<Question[]>
  {
    return this.http.get<Question[]>(this.baseUrl + 'Home/Questions');
  }
}
