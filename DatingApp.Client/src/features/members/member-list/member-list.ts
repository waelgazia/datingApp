import { Observable } from 'rxjs';
import { AsyncPipe } from '@angular/common';
import { Component, inject } from '@angular/core';

import { Member } from '../../../interfaces/models/Member';
import { MemberCard } from '../member-card/member-card';
import { MemberService } from '../../../core/services/member-service';

@Component({
  selector: 'app-member-list',
  imports: [ AsyncPipe, MemberCard ],
  templateUrl: './member-list.html',
  styleUrl: './member-list.css'
})

/*
  - user $ as a suffix when defining an observable (convention).
  - AsyncPipe handle the observable subscription, and when the component is disposed, it's going to
    automatically unsubscribe as well (this is not required for HttpClient, it is self disposable).
 */

export class MemberList {
  private memberService = inject(MemberService);
  protected members$: Observable<Member[]>;

  constructor() {
    this.members$ = this.memberService.getMembers();
  }
}
