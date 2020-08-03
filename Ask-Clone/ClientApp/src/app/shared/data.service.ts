import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Question } from './question';

@Injectable()
export class DataService {

  constructor(private http:HttpClient) { }

  private readonly baseurl = 'api/Questions/';

  authorizedGet():Observable<Question[]>
  {
    return this.http.get<Question[]>(this.baseurl);
  }

  unauthorizedGet(username:string):Observable<Question[]>
  {
    return this.http.get<Question[]>(this.baseurl+username);
  }

  post(username: string, question: Question)
  {
    return this.http.post(this.baseurl + username, question);
  }

  put(question:Question)
  {
    return this.http.put(this.baseurl+question.questionId.toString(),question);
  }

  delete(id:number)
  {
    return this.http.delete(this.baseurl+id.toString());
  }

  sort(questions:Question[])
  {
    questions.sort((a, b) => {
      const first = a.time;
      const second = b.time;

      if (first > second) return -1;
      else if (first < second) return 1;
      return 0;
    });
  }
}
