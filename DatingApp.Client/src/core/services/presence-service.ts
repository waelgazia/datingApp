import { inject, Injectable, signal } from '@angular/core';

import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';

import { ToastService } from './toast-service';
import { UserDto } from '../../interfaces/models/UserDto';
import { environment } from '../../environments/environment';
import { MessageDto } from '../../interfaces/models/MessageDto';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  private _hubUrl = environment.hubUrl;
  private _hubConnection?: HubConnection;
  private _toastService = inject(ToastService);

  onlineUsers = signal<string[]>([]);

  createHubConnection(user: UserDto) {
    if (this._hubConnection?.state === HubConnectionState.Connected) return;

    this._hubConnection = new HubConnectionBuilder()
      .withUrl(this._hubUrl + 'presence', {
        accessTokenFactory: () => user.token    // include the token in the request query
      })
      .withAutomaticReconnect() // to automatically reconnect if the connection is lost
      .build();

    this._hubConnection.start().catch(err => console.log(err));

    this._hubConnection.on('UserOnline', userId => {
      this.onlineUsers.update(users => [...users, userId]);
    });

    this._hubConnection.on('UserOffline', userId => {
      this.onlineUsers.update(users => users.filter(id => id !== userId));
    });

    this._hubConnection.on('GetOnlineUsers', onlineUsers => {
      this.onlineUsers.set(onlineUsers);
    });

    this._hubConnection.on('NewMessageReceived', (message: MessageDto) => {
      this._toastService.info(
        `${message.recipientDisplayName} has sent you a new message`,
        10000,
        message.senderImageUrl,
        `/members/${message.senderId}/messages`);
    })
  }

  stopHubConnection() {
    if (this._hubConnection?.state === HubConnectionState.Connected) {
      this._hubConnection.stop().catch(err => console.log(err));
    }
  }
}
