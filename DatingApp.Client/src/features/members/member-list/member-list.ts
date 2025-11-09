import { Component, inject, OnInit, signal, ViewChild } from '@angular/core';

import { MemberCard } from '../member-card/member-card';
import { FilterModal } from '../filter-modal/filter-modal';
import { STORAGE_KEY } from '../../../constants/storage-keys';
import { Paginator } from "../../../shared/paginator/paginator";
import { MemberDto } from '../../../interfaces/models/MemberDto';
import { MemberService } from '../../../core/services/member-service';
import { PaginatedResult } from '../../../interfaces/base/PaginatedResult';
import { MembersParameters } from '../../../interfaces/ResourceParameters/MembersParameters';

@Component({
  selector: 'app-member-list',
  imports: [MemberCard, Paginator, FilterModal],
  templateUrl: './member-list.html',
  styleUrl: './member-list.css'
})

/*
  - user $ as a suffix when defining an observable (convention).
  - AsyncPipe handle the observable subscription, and when the component is disposed, it's going to
    automatically unsubscribe as well (this is not required for HttpClient, it is self disposable).
 */
export class MemberList implements OnInit {
  @ViewChild('filterModal') filterModal!: FilterModal;

  // field to control the filter modal display message, this field allows updating the filter
  // message after filter modal submission not during updating the filter modal form.
  private updatedMembersParameters = new MembersParameters();

  private memberService = inject(MemberService);
  protected paginatedMembers = signal<PaginatedResult<MemberDto> | null>(null);
  protected membersParameters = new MembersParameters();

  constructor() {
    const filters = localStorage.getItem(STORAGE_KEY.FILTERS);
    if (filters) {
      this.membersParameters = JSON.parse(filters);
      this.updatedMembersParameters = JSON.parse(filters);
    }
  }

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers() {
    this.memberService.getMembers(this.membersParameters).subscribe({
      next: result => this.paginatedMembers.set(result)
    });
  }

  onPageChanged(event: { pageNumber: number, pageSize: number }) {
    this.membersParameters.pageNumber = event.pageNumber;
    this.membersParameters.pageSize = event.pageSize;
    this.loadMembers();
  }

  openFilterModal() {
    this.filterModal.open();
  }

  closeFilterModal() {
    console.log('Filter modal closed')
  }

  onFilterModalSubmitted(membersParameters: MembersParameters) {
    /* {...object} creates a deep copy of the object to prevent referencing the same object */
    this.membersParameters = {...membersParameters};
    this.updatedMembersParameters = {...membersParameters};

    this.loadMembers();
  }

  resetFilterModal() {
    this.membersParameters = new MembersParameters();
    this.updatedMembersParameters = new MembersParameters();

    this.loadMembers();
  }

  get filterDisplayMessage(): string {
    const defaultParameters = new MembersParameters();

    let filters: string[] = [];

    if (this.updatedMembersParameters.gender) {
      filters.push(this.updatedMembersParameters.gender + 's');
    } else {
      filters.push('Males, Females');
    }

    if (this.updatedMembersParameters.minAge !== defaultParameters.minAge
        || this.updatedMembersParameters.maxAge !== defaultParameters.maxAge) {
      filters.push(`ages ${this.updatedMembersParameters.minAge}-${this.updatedMembersParameters.maxAge}`);
    }

    filters.push(this.updatedMembersParameters.orderBy === 'lastActive'
      ? 'Recently active'
      : 'Newest members'
    );

    return filters.length > 0 ? `Selected: ${filters.join(' | ')}` : 'All members';
  }
}
