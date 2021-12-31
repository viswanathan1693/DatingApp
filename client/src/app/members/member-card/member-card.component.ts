import { Member } from './../../_models/member';
import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input()  member: Member | undefined;

  constructor() { }

  ngOnInit(): void {
  }

}
