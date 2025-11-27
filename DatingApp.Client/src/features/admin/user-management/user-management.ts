import { Component, ElementRef, inject, OnInit, signal, ViewChild } from '@angular/core';

import { ROLES } from '../../../constants/roles';
import { UserDto } from '../../../interfaces/models/UserDto';
import { AdminService } from '../../../core/services/admin-service';

@Component({
  selector: 'app-user-management',
  imports: [],
  templateUrl: './user-management.html',
  styleUrl: './user-management.css'
})
export class UserManagement implements OnInit {
  @ViewChild('updateUserRolesModal') updateUserRolesModal!: ElementRef<HTMLDialogElement>;

  private _adminService = inject(AdminService);
  protected adminEmail! : string;
  protected users = signal<UserDto[]>([]);
  protected selectedUser : UserDto | null = null;
  protected availableRoles = [ ROLES.MEMBER, ROLES.MODERATOR, ROLES.ADMIN ];

  ngOnInit(): void {
    this.adminEmail = this._adminService.getAdminEmail();
    this.getUserWithRoles();
  }

  getUserWithRoles() {
    this._adminService.getUserWithRoles().subscribe({
      next: users => this.users.set(users)
    })
  }

  openUpdateUserRolesModal(user: UserDto) {
    this.selectedUser = user;
    this.updateUserRolesModal.nativeElement.showModal();
  }

  toggleUserRole(event: Event, role: string) {
    if (!this.selectedUser) return;

    const isChecked = (event.target as HTMLInputElement).checked;
    if (isChecked) {
      this.selectedUser.roles.push(role);
    } else {
      this.selectedUser.roles = this.selectedUser.roles.filter(r => r !== role);
    }
  }

  updateUserRoles() {
    if (!this.selectedUser) return;
    this._adminService.updateUserRoles(this.selectedUser.id, this.selectedUser.roles)
      .subscribe({
        next: updatedRoles => {
          this.users.update(users => users.map(u => {
            if (u.id === this.selectedUser?.id) {
              u.roles = updatedRoles;
            }
            return u;
          }))
          this.updateUserRolesModal.nativeElement.close();
        },
        error: error => console.log('Failed to update roles.', error)
      });
  }
}
