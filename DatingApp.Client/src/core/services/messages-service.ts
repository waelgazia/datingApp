import { map, Observable } from 'rxjs';
import { inject, Injectable, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';

import { HttpUtils } from './http-utils';
import { AccountService } from './account-service';
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
  private _hubUrl = environment.hubUrl;

  private _httpClient = inject(HttpClient)
  private _accountService = inject(AccountService);
  private hubConnection? : HubConnection;

  messageThread = signal<MessageDto[]>([]);

  createHubConnection(otherUserId: string) {
    if (this.hubConnection?.state === HubConnectionState.Connected) return;

    const currentUser = this._accountService.currentUser();
    if (!currentUser) return;

    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this._hubUrl + 'messages?userId=' + otherUserId, {
        accessTokenFactory: () => currentUser.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch(err => console.log(err));

    this.hubConnection.on('ReceiveMessagesThread', (messages: MessageDto[]) => {
      this.messageThread.set(messages.map(message => ({
        ...message,
        currentUserIsSender: message.senderId !== otherUserId
      })));
    });

    this.hubConnection.on('NewMessage', (newMessage: MessageDto) => {
      newMessage.currentUserIsSender = newMessage.senderId === currentUser.id;
      this.messageThread.update(messages => [...messages, newMessage]);
    })
  }

  stopHubConnection() {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      this.hubConnection.stop().catch(err => console.log(err));
    }
  }

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
    return this.hubConnection?.invoke('SendMessage', { recipientId, content });
  }

  deleteMessage(messageId: string) {
    return this._httpClient.delete(this._baseUrl + `messages/${messageId}`)
  }
}
