import { HttpClient } from "@angular/common/http";
import { LoginUser } from "./login-user";
import { Injectable } from "@angular/core";
import { RegisterUser } from "./register-user";
import { SearchResult } from "./search-result";
import { Observable } from "rxjs";
import { Question } from "./question";
import { Passwords } from "./passwordsChange";


@Injectable()
export class UserService {
  constructor(
    private http: HttpClient) { }

  searchResult: SearchResult[] = [];

  private readonly baseUrl: string = "api/User/";

  signup(user: RegisterUser) {
    return this.http.post(this.baseUrl + 'Register', user);
  }

  login(user: LoginUser) {
    return this.http.post(this.baseUrl + 'login', user);
  }

  refreshToken(username: string)
  {
    return this.http.head(this.baseUrl + "RefreshToken/" + username);
  }


  logout() {
    return this.http.get(this.baseUrl + "Logout");
  }

  changePassword(passwords:Passwords){
    return this.http.post(this.baseUrl+'ChangePassword',
    {currentPassword:passwords.currentPassword,newPassword:passwords.newPassword});
  }

  search(searchValue: string): Observable<SearchResult[]> {
    return this.http.get<SearchResult[]>(this.baseUrl + 'search', { params: { username: searchValue } });
  }

  getHomeQuestions(): Observable<Question[]> {
    return this.http.get<Question[]>(this.baseUrl + 'Home/Questions');
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

}
