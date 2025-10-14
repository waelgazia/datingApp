import { Observable, tap } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';

import { Photo } from '../../interfaces/models/Photo';
import { Member } from '../../interfaces/models/Member';
import { environment } from '../../environments/environment';
import { EditableMember } from '../../interfaces/models/editableMember';

@Injectable({
  providedIn: 'root'
})
export class MemberService {
  private _httpClient = inject(HttpClient);
  private _baseUrl : string = environment.apiUrl;
  member = signal<Member | null>(null);
  editMode = signal<boolean>(false);

  getMembers(): Observable<Member[]> {
    return this._httpClient.get<Member[]>(this._baseUrl + 'members');
  }

  getMember(id: string): Observable<Member> {
    return this._httpClient.get<Member>(this._baseUrl + `members/${id}`).pipe(
        tap(member => {
          this.member.set(member)
        })
      );
  }

  getMemberPhotos(id: string): Observable<Photo[]> {
    return this._httpClient.get<Photo[]>(this._baseUrl + `members/${id}/photos`)
  }

  updateMember(member: EditableMember) {
    return this._httpClient.put(this._baseUrl + 'members', member);
  }

  uploadPhoto(file: File) {
    const formData = new FormData();
    formData.append('file', file);

    return this._httpClient.post<Photo>(this._baseUrl + `members/add-photo`, formData);
  }

  setMainPhoto(photo: Photo) {
    return this._httpClient.put(this._baseUrl + `members/set-main-photo/${photo.id}`, {});
  }

  deletePhoto(photoId: number) {
    return this._httpClient.delete(this._baseUrl + `members/delete-photo/${photoId}`);
  }
}
