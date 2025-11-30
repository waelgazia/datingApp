import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { AccountService } from '../services/account-service';

export const homeGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  if (!accountService.currentUser())
    return true;

  const router = inject(Router);
  return router.navigateByUrl('/members')
};
