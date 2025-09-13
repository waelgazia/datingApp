import { inject } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';

import { AccountService } from '../services/account-service';

// Interceptor to add the authorization token (if exist) before sending
// request to the server.
export const jwtInterceptor: HttpInterceptorFn = (req, next) => {

  const accountService = inject(AccountService)
  // when you copy a signal (in this case assigning accountService.currentUser to user)
  // it won't be signal anymore
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
