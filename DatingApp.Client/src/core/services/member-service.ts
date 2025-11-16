import { Observable, tap, map } from 'rxjs';
import { inject, Injectable, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

import { HttpUtils } from './http-utils';
import { STORAGE_KEY } from '../../constants/storage-keys';
import { PhotoDto } from '../../interfaces/models/PhotoDto';
import { environment } from '../../environments/environment';
import { MemberDto } from '../../interfaces/models/MemberDto';
import { PaginatedResult } from '../../interfaces/base/PaginatedResult';
import { EditableMemberDto } from '../../interfaces/models/EditableMemberDto';
import { MembersParameters } from '../../interfaces/ResourceParameters/MembersParameters';

@Injectable({
  providedIn: 'root'
})
export class MemberService {
  private _httpClient = inject(HttpClient);
  private _baseUrl : string = environment.apiUrl;
  member = signal<MemberDto | null>(null);
  editMode = signal<boolean>(false);

  getMembers(membersParameters: MembersParameters): Observable<PaginatedResult<MemberDto>> {
    let queryParameters = new HttpParams()
      .set('pageNumber', membersParameters.pageNumber)
      .set('pageSize', membersParameters.pageSize)
      .set('minAge', membersParameters.minAge)
      .set('maxAge', membersParameters.maxAge)
      .set('orderBy', membersParameters.orderBy);

    if (membersParameters.gender) {
      queryParameters = queryParameters.append('gender', membersParameters.gender);
    }

    return this._httpClient
      .get<MemberDto[]>(this._baseUrl + 'members', {
        params: queryParameters,
        observe: 'response'
      })
      .pipe(
        map(res => HttpUtils.GetPaginatedResult<MemberDto>(res)),
        tap(() => {
          localStorage.setItem(STORAGE_KEY.FILTERS, JSON.stringify(membersParameters));
        })
      );
  }

  getMember(id: string): Observable<MemberDto> {
    return this._httpClient.get<MemberDto>(this._baseUrl + `members/${id}`).pipe(
        tap(member => {
          this.member.set(member)
        })
      );
  }

  getMemberPhotos(id: string): Observable<PhotoDto[]> {
    return this._httpClient.get<PhotoDto[]>(this._baseUrl + `members/${id}/photos`)
  }

  updateMember(member: EditableMemberDto) {
    return this._httpClient.put(this._baseUrl + 'members', member);
  }

  uploadPhoto(file: File) {
    const formData = new FormData();
    formData.append('file', file);

    return this._httpClient.post<PhotoDto>(this._baseUrl + `members/add-photo`, formData);
  }

  setMainPhoto(photo: PhotoDto) {
    return this._httpClient.put(this._baseUrl + `members/set-main-photo/${photo.id}`, {});
  }

  deletePhoto(photoId: number) {
    return this._httpClient.delete(this._baseUrl + `members/delete-photo/${photoId}`);
  }
}
