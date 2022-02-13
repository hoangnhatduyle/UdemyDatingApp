import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({ //decorator
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Dating App';
  users: any; //user can be any type (strings, numbers, ...)

  constructor(private http: HttpClient) { //dependency injection

  }
  ngOnInit() {
    this.getUsers();
  }

  getUsers() {
    this.http.get('https://localhost:5001/api/users').subscribe(response => {
      this.users = response;
    }, error => {
      console.log(error);
    })
    //get method returns observable - way of Angular to handle asynchronous code  
  }
}
