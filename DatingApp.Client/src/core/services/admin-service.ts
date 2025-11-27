import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { UserDto } from '../../interfaces/models/UserDto';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private _baseUrl = environment.apiUrl;
  private _httpClient = inject(HttpClient);

  getUserWithRoles() {
    return this._httpClient.get<UserDto[]>(this._baseUrl + 'admins/users-with-roles');
  }

  updateUserRoles(userId: string, roles: string[]) {
    return this._httpClient
      .post<string[]>(this._baseUrl + `admins/edit-roles/${userId}?roles=${roles}`, {});
  }

  getAdminEmail() {
    return environment.adminEmail;
  }
}
