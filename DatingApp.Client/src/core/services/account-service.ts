import { tap } from 'rxjs';
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
    localStorage.removeItem(STORAGE_KEY.USER);
    localStorage.removeItem(STORAGE_KEY.FILTERS);
    this.currentUser.set(null);
    this._likesService.clearLikeIds();
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
    user.roles = this.getRolesFromToken(user);

    localStorage.setItem(STORAGE_KEY.USER, JSON.stringify(user));
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
