import { Component, inject, OnInit, signal } from '@angular/core';

import { Paginator } from "../../shared/paginator/paginator";
import { MemberDto } from '../../interfaces/models/MemberDto';
import { MemberCard } from "../members/member-card/member-card";
import { LikesService } from '../../core/services/likes-service';
import { LIKE_PREDICATES } from '../../constants/like-predicates';
import { PaginatedResult } from '../../interfaces/base/PaginatedResult';
import { LikesParameters } from '../../interfaces/ResourceParameters/LikesParameters';

@Component({
  selector: 'app-lists',
  imports: [MemberCard, Paginator],
  templateUrl: './lists.html',
  styleUrl: './lists.css'
})
export class Lists implements OnInit {
  private _likesService = inject(LikesService);

  protected paginatedMembers = signal<PaginatedResult<MemberDto> | null>(null);
  protected likesParameters = new LikesParameters();
  protected tabs = [
    { label: 'Liked', value: LIKE_PREDICATES.LIKED },
    { label: 'Liked me', value: LIKE_PREDICATES.LIKED_BY },
    { label: 'Mutual', value: LIKE_PREDICATES.MUTUAL }
  ];

  ngOnInit(): void {
    this.loadLikes();
  }

  setPredicate(predicate: string) {
    if (this.likesParameters.predicate !== predicate) {
      this.likesParameters = new LikesParameters();
      this.likesParameters.predicate = predicate;

      this.loadLikes();
    }
  }

  loadLikes() {
    this._likesService.getLikes(this.likesParameters).subscribe({
      next: result => this.paginatedMembers.set(result)
    });
  }

  onPageChanged(event: { pageNumber: number, pageSize: number }) {
    this.likesParameters.pageNumber = event.pageNumber;
    this.likesParameters.pageSize = event.pageSize;
    this.loadLikes();
  }
}
