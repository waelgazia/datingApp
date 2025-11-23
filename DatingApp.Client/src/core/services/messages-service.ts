import { map, Observable } from 'rxjs';
import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

import { HttpUtils } from './http-utils';
import { environment } from '../../environments/environment';
import { MessageDto } from '../../interfaces/models/MessageDto';
import { QUERY_PARAMETERS } from '../../constants/query-parameters';
import { PaginatedResult } from '../../interfaces/base/PaginatedResult';
import { MessagesParameters } from '../../interfaces/ResourceParameters/MessagesParameters';

@Injectable({
  providedIn: 'root'
})
export class MessagesService {
  private _baseUrl = environment.apiUrl;
  private _httpClient = inject(HttpClient)

  getMessages(messagesParameters: MessagesParameters) : Observable<PaginatedResult<MessageDto>> {
    let queryParameters = new HttpParams()
      .set(QUERY_PARAMETERS.PAGE_NUMBER, messagesParameters.pageNumber)
      .set(QUERY_PARAMETERS.PAGE_SIZE, messagesParameters.pageSize)
      .set('container', messagesParameters.container);

    return this._httpClient
      .get<MessageDto[]>(this._baseUrl + 'messages', {
        params: queryParameters,
        observe: 'response'
      })
      .pipe(
        map(res => HttpUtils.GetPaginatedResult(res))
      );
  }

  // recipientId is the one requesting the messages on its portal
  // it is not necessary the one who received it.
  getMessageThread(otherMemberId: string) : Observable<MessageDto[]> {
    return this._httpClient.get<MessageDto[]>(this._baseUrl + `messages/thread/${otherMemberId}`);
  }

  sendMessage(recipientId: string, content: string) {
    return this._httpClient.post<MessageDto>(this._baseUrl + 'messages', { recipientId, content });
  }

  deleteMessage(messageId: string) {
    return this._httpClient.delete(this._baseUrl + `messages/${messageId}`)
  }
}
