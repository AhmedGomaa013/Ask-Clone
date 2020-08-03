import { HttpClient } from "@angular/common/http";
import { LoginUser } from "./login-user";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { RegisterUser } from "./register-user";
import { SearchResult } from "./search-result";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";

@Injectable()
export class UserService {
    constructor(private http:HttpClient, private router:Router) {}

    searchResult:SearchResult[]=[];
    
    private readonly baseUrl:string = "api/User/";
    
    login(user:LoginUser)
    {
        return this.http.post(this.baseUrl+'login',user);
    }

    signup(user:RegisterUser)
    {
        return this.http.post(this.baseUrl+'Register',user);
    }

  search(searchValue: string): Observable<SearchResult[]>
  {
    return this.http.get<SearchResult[]>(this.baseUrl+'search', { params: { username: searchValue } });
    }
}
