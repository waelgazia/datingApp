import { tap } from 'rxjs';
import { inject, Injectable } from '@angular/core';

import { AccountService } from './account-service';

@Injectable({
  providedIn: 'root'
})
export class InitService {
  private _accountService = inject(AccountService);

  // this method will run before Angular load any components.
  // for this to work, we need to return an observable
  init() {
    return this._accountService.refreshToken().pipe(
      tap(user => {
        if (user) {
          this._accountService.setCurrentUser(user);
          this._accountService.startTokenRefreshInterval();
        }
      })
    )
  }
}
