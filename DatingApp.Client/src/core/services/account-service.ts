import { tap } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';

import { User } from '../../interfaces/models/User';
import { LoginVM } from '../../interfaces/models/LoginVM';
import { RegisterVM } from '../../interfaces/models/RegisterVM';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private _httpClient = inject(HttpClient);
  private _baseUrl : string = environment.apiUrl;

  currentUser = signal<User | null>(null);

  login(loginVM: LoginVM) {
    return this._httpClient.post<User>(this._baseUrl + 'accounts/login', loginVM).pipe(
      tap(user => {
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUser.set(null);
  }

  register(registerVM: RegisterVM) {
    return this._httpClient.post<User>(this._baseUrl + 'accounts/register', registerVM).pipe(
      tap(user => {
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  setCurrentUser(user: User) {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUser.set(user);
  }
}
