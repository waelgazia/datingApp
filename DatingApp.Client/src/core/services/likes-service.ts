import { map, Observable } from 'rxjs';
import { inject, Injectable, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

import { HttpUtils } from './http-utils';
import { environment } from '../../environments/environment';
import { MemberDto } from '../../interfaces/models/MemberDto';
import { QUERY_PARAMETERS } from '../../constants/query-parameters';
import { PaginatedResult } from '../../interfaces/base/PaginatedResult';
import { LikesParameters } from '../../interfaces/ResourceParameters/LikesParameters';

@Injectable({
  providedIn: 'root'
})
export class LikesService {
  private _httpClient = inject(HttpClient);
  private _baseUrl: string = environment.apiUrl;

  userLikeIds = signal<string[]>([]);

  toggleLike(targetMemberId: string) {
    return this._httpClient.post(this._baseUrl + `likes/${targetMemberId}`, {}).subscribe({
      next: () => {
        if (this.userLikeIds().includes(targetMemberId)) {
          this.userLikeIds.update(ids => ids.filter(x => x !== targetMemberId))
        } else {
          this.userLikeIds.update(ids => ([...ids, targetMemberId]));
        }
      }
    });
  }

  getLikes(likesParameters: LikesParameters): Observable<PaginatedResult<MemberDto>> {
    let queryParameters = new HttpParams()
      .set(QUERY_PARAMETERS.PAGE_NUMBER, likesParameters.pageNumber)
      .set(QUERY_PARAMETERS.PAGE_SIZE, likesParameters.pageSize)
      .set('predicate', likesParameters.predicate);

    return this._httpClient
      .get<MemberDto[]>(this._baseUrl + 'likes', {
        params: queryParameters,
        observe: 'response'
      })
      .pipe(
        map(res => HttpUtils.GetPaginatedResult<MemberDto>(res))
      )
  }

  getLikeIds() {
    return this._httpClient.get<string[]>(this._baseUrl + `likes/list`).subscribe({
      next: ids => this.userLikeIds.set(ids)
    });
  }

  clearLikeIds() {
    this.userLikeIds.set([]);
  }
}
