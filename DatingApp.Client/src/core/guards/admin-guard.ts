import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';

import { ROLES } from '../../constants/roles';
import { ToastService } from '../services/toast-service';
import { AccountService } from '../services/account-service';

export const adminGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const toastService = inject(ToastService);

  if (accountService.currentUser()?.roles.includes(ROLES.ADMIN)
    || accountService.currentUser()?.roles.includes(ROLES.MODERATOR)) {
    return true;
  } else {
    toastService.error('Enter this area, you cannot')
    return false;
  }
};
