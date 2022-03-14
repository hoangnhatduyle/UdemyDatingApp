import { Component, Input, OnInit } from '@angular/core';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @Input() messages: Member[];

  constructor() { }

  ngOnInit(): void {
    
  }
}
