import { Component, inject } from '@angular/core';

import { UserManagement } from './user-management/user-management';
import { AccountService } from '../../core/services/account-service';
import { PhotoManagement } from './photo-management/photo-management';

@Component({
  selector: 'app-admin',
  imports: [UserManagement, PhotoManagement],
  templateUrl: './admin.html',
  styleUrl: './admin.css'
})
export class Admin {
  protected accountService = inject(AccountService);

  activeTab = 'photos';
  tabs = [
    { label: 'Photo moderation', value: 'photos' },
    { label: 'User management', value: 'roles' }
  ]

  setTab(tab: string) {
    this.activeTab = tab;
  }
}
