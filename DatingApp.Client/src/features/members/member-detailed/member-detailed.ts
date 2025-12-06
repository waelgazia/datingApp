import { filter } from 'rxjs';
import { NgClass } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { AgePipe } from '../../../core/pipes/age-pipe';
import { MemberService } from '../../../core/services/member-service';
import { AccountService } from '../../../core/services/account-service';
import { PresenceService } from '../../../core/services/presence-service';

@Component({
  selector: 'app-member-detailed',
  imports: [RouterLink, RouterLinkActive, RouterOutlet, AgePipe, NgClass],
  templateUrl: './member-detailed.html',
  styleUrl: './member-detailed.css'
})
export class MemberDetailed implements OnInit {
  private _accountService = inject(AccountService);
  private _route = inject(ActivatedRoute);
  private _router = inject(Router);

  protected memberService = inject(MemberService);
  protected _presenceService = inject(PresenceService);
  protected title = signal<string | undefined>('Profile');

  protected isCurrentUser = computed(() => {
    return this._accountService.currentUser()?.id === this.memberService.member()?.id;
  });
  protected editModeEnabled = computed(() => this.memberService.editMode());

  ngOnInit(): void {
    // get data from member-resolver
    this.title.set(this._route.firstChild?.snapshot?.title);

    // subscribe to NavigationEnd event to update title accordingly
    this._router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe({
      next: () => {this.title.set(this._route.firstChild?.snapshot?.title)}
    })
  }

  goBack() {
    this._router.navigateByUrl('/members');
  }

  toggleEdit() {
    this.memberService.editMode.set(!this.memberService.editMode());
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
