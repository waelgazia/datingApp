import { DatePipe } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { Component, computed, HostListener, inject, OnDestroy, OnInit, ViewChild } from '@angular/core';

import { Member } from '../../../interfaces/models/Member';
import { ToastService } from '../../../core/services/toast-service';
import { MemberService } from '../../../core/services/member-service';
import { EditableMember } from '../../../interfaces/models/editableMember';
import { AccountService } from '../../../core/services/account-service';

@Component({
  selector: 'app-member-profile',
  imports: [ DatePipe, FormsModule ],
  templateUrl: './member-profile.html',
  styleUrl: './member-profile.css'
})
export class MemberProfile implements OnInit, OnDestroy {
  // @ViewChild allows to access a form created in the template by its template reference in the component.ts
  @ViewChild('editForm') editForm? : NgForm;

  // subscript to the browser (host listener) unload event to prevent navigation when the form is edited.
  @HostListener('window:beforeunload', ['$event']) notify($event:BeforeUnloadEvent) {
    if (this.editForm?.dirty) {
      $event.preventDefault();
    }
  }

  private _accountService = inject(AccountService);
  protected memberService = inject(MemberService);
  private _toastService = inject(ToastService);

  protected editModeEnabled = computed(() => this.memberService.editMode());
  protected editableMember : EditableMember = {
    displayName: '',
    description: '',
    city: '',
    country: '',
  };

  ngOnInit(): void {
    this.editableMember = {
      displayName: this.memberService.member()?.displayName || '',
      description: this.memberService.member()?.description || '',
      city: this.memberService.member()?.city || '',
      country: this.memberService.member()?.country || '',
    };
  }

  updateProfile() {
    if (!this.editModeEnabled()) return;
    // The spread operator (...) overwrites the values of member(), with the ones in editableMember
    const updatedMember = { ...this.memberService.member(), ...this.editableMember };

    this.memberService.updateMember(this.editableMember).subscribe({
      next: () => {
        // updated account user data if needed
        const currentUser = this._accountService.currentUser();
        if (currentUser && currentUser.displayName !== updatedMember.displayName) {
          currentUser.displayName = updatedMember.displayName
          this._accountService.currentUser.set(currentUser);
        }

        this._toastService.success('Profile updated successfully');
        this.memberService.editMode.set(false);
        this.memberService.member.set(updatedMember as Member);

        // reset the form to update its state by calling reset (this updates the dirty flag to false)
        // so then our can deactivate guard won't interrupt moving around to different component.
        this.editForm?.reset(updatedMember);
      }
    })
  }

  ngOnDestroy(): void {
    if (this.editModeEnabled()) {
      this.memberService.editMode.set(false);
    }
  }
}
