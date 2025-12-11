import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Component, effect, ElementRef, inject, OnDestroy, OnInit, signal, ViewChild } from '@angular/core';

import { TimeAgoPipe } from '../../../core/pipes/time-ago-pipe';
import { MemberService } from '../../../core/services/member-service';
import { MessagesService } from '../../../core/services/messages-service';
import { PresenceService } from '../../../core/services/presence-service';

@Component({
  selector: 'app-member-messages',
  imports: [DatePipe, TimeAgoPipe, FormsModule],
  templateUrl: './member-messages.html',
  styleUrl: './member-messages.css'
})
export class MemberMessages implements OnInit, OnDestroy {
  @ViewChild('messageEndRef') messageEndRef!: ElementRef;

  private _memberService = inject(MemberService);
  private _route = inject(ActivatedRoute);

  protected messagesService = inject(MessagesService);
  protected presenceService = inject(PresenceService);
  protected messageContent = signal<string>('');

  constructor() {
    // The code inside effect() runs immediately once when the component is constructed,
    // and every time the messages signal changes, meaning you call set, update, or mutate on it.
    effect(() => {
      const currentMessages = this.messagesService.messageThread();
      if (currentMessages.length > 0) {
        this.scrollToBottom();
      }
    })
  }

  ngOnInit(): void {
    this._route.parent?.paramMap.subscribe({
      next: params => {
        const userToMessageId = params.get('id');
        if (!userToMessageId) {
          throw new Error("Cannot connect to message hub");
        }

        this.messagesService.createHubConnection(userToMessageId);
      }
    })
  }

  ngOnDestroy(): void {
    this.messagesService.stopHubConnection();
  }

  sendMessage() {
    const recipientId = this._memberService.member()?.id;
    if (!recipientId || !this.messageContent()) return;

    this.messagesService.sendMessage(recipientId, this.messageContent())
      ?.then(() => {
        this.messageContent.set('');
      });
  }

  scrollToBottom() {
    /*
      Effect() run before Angular completes DOM updates. setTimeout() delays the scroll operation until
      after the DOM is fully rendered, preventing undefined references and incorrect scroll behavior.
    */
    setTimeout(() => {
      if (this.messageEndRef){
        this.messageEndRef.nativeElement.scrollIntoView({ behavior: 'smooth' })
      }
    })
  }
}
