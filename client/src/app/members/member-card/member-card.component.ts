import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() member : Member;
  @Output() reloadMemberlike = new EventEmitter();

  constructor(private memberSerive: MembersService, private toastr: ToastrService, public presence: PresenceService) { }

  ngOnInit(): void {
  }

  addLike(member: Member) {
    this.memberSerive.addLike(member.username).subscribe((response) => {
      if (response) {
        this.toastr.success("You have liked " + member.knownAs);
      }
      else {
        this.toastr.success("You have unliked " + member.knownAs);
      }
      this.reloadLikes();
    })
  }

  reloadLikes() {
    this.reloadMemberlike.emit(true);
  }
}
