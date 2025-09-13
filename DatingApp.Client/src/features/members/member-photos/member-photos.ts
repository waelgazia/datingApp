import { Observable } from 'rxjs';
import { AsyncPipe } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { Component, inject } from '@angular/core';

import { Photo } from '../../../interfaces/Photo';
import { MemberService } from '../../../core/services/member-service';

@Component({
  selector: 'app-member-photos',
  imports: [AsyncPipe],
  templateUrl: './member-photos.html',
  styleUrl: './member-photos.css'
})
export class MemberPhotos {
  private _route = inject(ActivatedRoute)
  private _memberService = inject(MemberService)

  protected photos$? : Observable<Photo[]>;

  constructor() {
    const memberId = this._route.parent?.snapshot.paramMap.get('id');
    if (memberId) {
      this.photos$ = this._memberService.getMemberPhotos(memberId)
    }
  }

  // mock a photo list
  get photoMocks() {
    // The parentheses () around {} are for telling JS you’re returning an object literal,
    // not starting a block of code.
    return Array.from({ length: 20 }, (_, i) => ({
      url: './user.png'
    }));
  }
}
