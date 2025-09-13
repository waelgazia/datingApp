import { filter } from 'rxjs';
import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { Member } from '../../../interfaces/Member';
import { AgePipe } from '../../../core/pipes/age-pipe';

@Component({
  selector: 'app-member-detailed',
  imports: [RouterLink, RouterLinkActive, RouterOutlet, AgePipe],
  templateUrl: './member-detailed.html',
  styleUrl: './member-detailed.css'
})
export class MemberDetailed implements OnInit {
  protected member = signal<Member | undefined>(undefined);
  private _route = inject(ActivatedRoute);
  private _router = inject(Router);
  protected title = signal<string | undefined>('Profile');

  ngOnInit(): void {
    // get data from member-resolver
    this._route.data.subscribe({
      next: data => this.member.set(data['member'])
    })
    this.title.set(this._route.firstChild?.snapshot?.title);

    // subscribe to NavigationEnd event to update title accordingly
    this._router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe({
      next: () => {this.title.set(this._route.firstChild?.snapshot?.title)}
    })
  }

  /*
    ====> ANOTHER APPROACH TO LOAD DATA WITHOUT USING RESOLVER <====
    protected member$? : Observable<Member>;

    ngOnInit(): void {
      this.member$ = this.loadMember();
    }

    loadMember() {
      const memberId = this._route.snapshot.paramMap.get('id');
      if (!memberId) return;

      return this._membersService.getMember(memberId);
    }
  */
}
