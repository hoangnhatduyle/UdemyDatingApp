import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({   //can be injected to other componenets or other services
  providedIn: 'root'  //automatically added to the app module. This is a singleton
})
export class AccountService {
  baseUrl = 'https://localhost:5001/api/';

  constructor(private http: HttpClient) { }

  login(model: any) {
    return this.http.post(this.baseUrl + 'account/login', model);
  }
}
