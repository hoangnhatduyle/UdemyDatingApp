import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { User } from './_models/user';
import { AccountService } from './_services/account.service';
import { PresenceService } from './_services/presence.service';

@Component({ //decorator
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Dating App';
  users: any; //user can be any type (strings, numbers, ...)

  constructor(private accountService: AccountService, private presenceService: PresenceService) { //dependency injection

  }
  ngOnInit() {
    this.setCurrentUser();
  }

  setCurrentUser() {
    const user: User = JSON.parse(localStorage.getItem('user'));    //set the new user logged in to store in local storage
    if (user) {
      this.accountService.setCurrentUser(user);
      this.presenceService.createHubConnection(user);
    }
  }
}
