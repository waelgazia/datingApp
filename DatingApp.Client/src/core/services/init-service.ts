import { Observable, of } from 'rxjs';
import { inject, Injectable } from '@angular/core';

import { LikesService } from './likes-service';
import { AccountService } from './account-service';
import { STORAGE_KEY } from '../../constants/storage-keys';

@Injectable({
  providedIn: 'root'
})
export class InitService {
  private _accountService = inject(AccountService);
  private _likesService = inject(LikesService);

  // this method will run before Angular load any components.
  // for this to work, we need to return an observable
  init() : Observable<null> {
    const userString = localStorage.getItem(STORAGE_KEY.USER);
    if (!userString) {
      return of(null);
    }

    const user = JSON.parse(userString);
    this._accountService.currentUser.set(user);
    this._likesService.getLikeIds();

    return of(null);   /* it used to be ObservableOf() */
  }
}
