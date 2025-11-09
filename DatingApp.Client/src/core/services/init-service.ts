import { inject, Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

import { AccountService } from './account-service';
import { STORAGE_KEY } from '../../constants/storage-keys';

@Injectable({
  providedIn: 'root'
})
export class InitService {
  private _accountService = inject(AccountService);

  // this method will run before Angular load any components.
  // for this to work, we need to return an observable
  init() : Observable<null> {
    const userString = localStorage.getItem(STORAGE_KEY.USER);
    if (!userString) {
      return of(null);
    }

    const user = JSON.parse(userString);
    this._accountService.currentUser.set(user);

    return of(null);   /* it used to be ObservableOf() */
  }
}
