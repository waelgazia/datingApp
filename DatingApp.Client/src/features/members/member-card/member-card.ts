import { RouterLink } from '@angular/router';
import { Component, computed, inject, input } from '@angular/core';

import { AgePipe } from '../../../core/pipes/age-pipe';
import { MemberDto } from '../../../interfaces/models/MemberDto';
import { LikesService } from '../../../core/services/likes-service';
import { PresenceService } from '../../../core/services/presence-service';

@Component({
  selector: 'app-member-card',
  imports: [RouterLink, AgePipe],
  templateUrl: './member-card.html',
  styleUrl: './member-card.css'
})
export class MemberCard {
  private _likesService = inject(LikesService);
  private _presenceService = inject(PresenceService);

  // determine if the current user has liked this member
  protected hasLiked = computed(() => this._likesService.userLikeIds().includes(this.member().id));
  protected isOnline = computed(() => this._presenceService.onlineUsers().includes(this.member().id));

  member = input.required<MemberDto>();

  toggleLike(event: Event) {
    // stop navigation to the member detail when click on the card
    event.stopPropagation();

    this._likesService.toggleLike(this.member().id).subscribe({
      next: () => {
        if (this.hasLiked()) {
          this._likesService.userLikeIds.update(ids => ids.filter(x => x !== this.member().id))
        } else {
          this._likesService.userLikeIds.update(ids => ([...ids, this.member().id]));
        }
      }
    })
  }
}
