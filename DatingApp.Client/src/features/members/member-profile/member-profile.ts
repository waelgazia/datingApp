import { DatePipe } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { Component, inject, OnInit, signal } from '@angular/core';

import { Member } from '../../../interfaces/Member';

@Component({
  selector: 'app-member-profile',
  imports: [ DatePipe ],
  templateUrl: './member-profile.html',
  styleUrl: './member-profile.css'
})
export class MemberProfile implements OnInit {
  private _route = inject(ActivatedRoute)
  protected member = signal<Member | undefined>(undefined);

  ngOnInit(): void {
    this._route.parent?.data.subscribe({
      next: data => this.member.set(data['member'])
    })
  }
}
