import { inject } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';

import { AccountService } from '../services/account-service';

// Interceptor to add the authorization token (if exist) before sending
// request to the server.
export const jwtInterceptor: HttpInterceptorFn = (req, next) => {

  const accountService = inject(AccountService)
  // copying a signal (e.g., const user = accountService.currentUser())
  // the 'user' won't be signal anymore
  const user = accountService.currentUser();

  if (user) {
    // the req is immutable so, we need to clone it before editing it
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${user.token}`
      }
    })
  }

  return next(req);
};
