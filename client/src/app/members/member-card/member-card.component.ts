import { ToastrService } from 'ngx-toastr';
import { MembersService } from 'src/app/_services/members.service';
import { Member } from './../../_models/member';
import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input()  member: Member | undefined;

  constructor(private memberServive: MembersService, private toastr: ToastrService) { }

  ngOnInit(): void {
  }

  addLike(member: Member){
    this.memberServive.addLike(member.username).subscribe(()=> {
      this.toastr.success('Yu have liked ' + member.knownAs);
    });
  }

}
