import { tap } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';

import { UserDto } from '../../interfaces/models/UserDto';
import { LoginDto } from '../../interfaces/models/LoginDto';
import { RegisterDto } from '../../interfaces/models/RegisterDto';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private _httpClient = inject(HttpClient);
  private _baseUrl : string = environment.apiUrl;

  currentUser = signal<UserDto | null>(null);

  login(loginDto: LoginDto) {
    return this._httpClient.post<UserDto>(this._baseUrl + 'accounts/login', loginDto).pipe(
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

  register(registerDto: RegisterDto) {
    return this._httpClient.post<UserDto>(this._baseUrl + 'accounts/register', registerDto).pipe(
      tap(user => {
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  setCurrentUser(user: UserDto) {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUser.set(user);
  }
}
