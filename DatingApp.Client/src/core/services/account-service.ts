import { Observable, tap } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';

import { ROLES } from '../../constants/roles';
import { LikesService } from './likes-service';
import { UserDto } from '../../interfaces/models/UserDto';
import { STORAGE_KEY } from '../../constants/storage-keys';
import { LoginDto } from '../../interfaces/models/LoginDto';
import { environment } from '../../environments/environment';
import { RegisterDto } from '../../interfaces/models/RegisterDto';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private _httpClient = inject(HttpClient);
  private _likesService = inject(LikesService);
  private _baseUrl : string = environment.apiUrl;

  currentUser = signal<UserDto | null>(null);

  // To use cookie with HTTP request in register and login we need to set withCredential to
  // true so that we can get the refresh token cookie back from the API and save it as cookie
  register(registerDto: RegisterDto) {
    return this._httpClient.post<UserDto>(
      this._baseUrl + 'accounts/register', registerDto, { withCredentials: true })
    .pipe(
      tap(user => {
        if (user) {
          this.setCurrentUser(user);
          this.startTokenRefreshInterval();
        }
      })
    );
  }

  login(loginDto: LoginDto) {
    return this._httpClient.post<UserDto>(this._baseUrl + 'accounts/login', loginDto, { withCredentials: true })
    .pipe(
      tap(user => {
        if (user) {
          this.setCurrentUser(user);
          this.startTokenRefreshInterval();
        }
      })
    );
  }

  logout() {
    return this._httpClient.post(this._baseUrl + 'accounts/logout', {}, { withCredentials: true })
      .pipe(
        tap({
          next: () => {
            localStorage.removeItem(STORAGE_KEY.FILTERS);
            this.currentUser.set(null);
            this._likesService.clearLikeIds();
          }
        })
      )
  }


  refreshToken() : Observable<UserDto> {
    return this._httpClient.post<UserDto>(
      this._baseUrl + 'accounts/refresh-token', {}, { withCredentials: true })
  }

  startTokenRefreshInterval() {
    setInterval(() => {
      this._httpClient.post<UserDto>(this._baseUrl + 'accounts/refresh-token', {}, { withCredentials: true })
        .subscribe({
          next: user => this.setCurrentUser(user),
          error: () => this.logout()
        })
    }, 7 * 60 * 1000);    // refresh the token each 7 minutes
  }

  setCurrentUser(user: UserDto) {
    user.roles = this.getRolesFromToken(user);

    this.currentUser.set(user);
    this._likesService.getLikeIds();
  }

  private getRolesFromToken(user: UserDto): string[] {
    const payload = user.token.split('.')[1];
    const decoded = atob(payload);
    const jsonPayload = JSON.parse(decoded);

    return Array.isArray(jsonPayload.role)
      ? jsonPayload.role
      : [ jsonPayload.role ];
  }

  isCurrentUserAdmin() {
    return this.currentUser()?.roles.includes(ROLES.ADMIN);
  }

  isCurrentUserModerator() {
    return this.currentUser()?.roles.includes(ROLES.MODERATOR);
  }
}
