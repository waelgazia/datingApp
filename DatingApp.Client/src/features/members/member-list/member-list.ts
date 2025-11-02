import { Component, inject, OnInit, signal } from '@angular/core';

import { MemberCard } from '../member-card/member-card';
import { Paginator } from "../../../shared/paginator/paginator";
import { MemberDto } from '../../../interfaces/models/MemberDto';
import { MemberService } from '../../../core/services/member-service';
import { PaginatedResult } from '../../../interfaces/base/PaginatedResult';

@Component({
  selector: 'app-member-list',
  imports: [MemberCard, Paginator],
  templateUrl: './member-list.html',
  styleUrl: './member-list.css'
})

/*
  - user $ as a suffix when defining an observable (convention).
  - AsyncPipe handle the observable subscription, and when the component is disposed, it's going to
    automatically unsubscribe as well (this is not required for HttpClient, it is self disposable).
 */
export class MemberList implements OnInit {
  private memberService = inject(MemberService);
  protected paginatedMembers = signal<PaginatedResult<MemberDto> | null>(null);
  pageNumber = 1;
  pageSize = 5;

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers() {
    this.memberService.getMembers(this.pageNumber, this.pageSize).subscribe({
      next: result => this.paginatedMembers.set(result)
    });
  }

  onPageChanged(event: { pageNumber: number, pageSize: number }) {
    this.pageNumber = event.pageNumber;
    this.pageSize = event.pageSize;
    this.loadMembers();
  }
}
