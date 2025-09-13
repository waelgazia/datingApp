import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { Photo } from '../../interfaces/Photo';
import { Member } from '../../interfaces/Member';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class MemberService {
  private _httpClient = inject(HttpClient);
  private _baseUrl: string = environment.apiUrl;

  getMembers(): Observable<Member[]> {
    return this._httpClient.get<Member[]>(this._baseUrl + 'members');
  }

  getMember(id: string): Observable<Member> {
    return this._httpClient.get<Member>(this._baseUrl + `members/${id}`);
  }

  getMemberPhotos(id: string): Observable<Photo[]> {
    return this._httpClient.get<Photo[]>(this._baseUrl + `members/${id}/photos`)
  }
}
