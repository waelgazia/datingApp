import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Component, effect, ElementRef, inject, OnInit, signal, ViewChild } from '@angular/core';

import { TimeAgoPipe } from '../../../core/pipes/time-ago-pipe';
import { MessageDto } from '../../../interfaces/models/MessageDto';
import { MemberService } from '../../../core/services/member-service';
import { MessagesService } from '../../../core/services/messages-service';

@Component({
  selector: 'app-member-messages',
  imports: [DatePipe, TimeAgoPipe, FormsModule],
  templateUrl: './member-messages.html',
  styleUrl: './member-messages.css'
})
export class MemberMessages implements OnInit {
  @ViewChild('messageEndRef') messageEndRef!: ElementRef;

  private _memberService = inject(MemberService);
  private _messagesService = inject(MessagesService);

  protected messages = signal<MessageDto[]>([]);
  protected messageContent = '';

  constructor() {
    // The code inside effect() runs immediately once when the component is constructed,
    // and every time the messages signal changes, meaning you call set, update, or mutate on it.
    effect(() => {
      const currentMessages = this.messages();
      if (currentMessages.length > 0) {
        this.scrollToBottom();
      }
    })
  }

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages() {
    const memberId = this._memberService.member()?.id;
    if (memberId) {
      this._messagesService.getMessageThread(memberId).subscribe({
        next: messages => this.messages.set(
          messages.map(message => ({
            ...message,
            currentUserIsSender: message.senderId !== memberId
          }))
        )
      })
    }
  }

  sendMessage() {
    const recipientId = this._memberService.member()?.id;
    if (!recipientId) return;

    this._messagesService.sendMessage(recipientId, this.messageContent).subscribe({
      next: message => {
        console.log(message);

        this.messages.update(messages => {
          message.currentUserIsSender = true;
          return [...messages, message]
        });

        this.messageContent = '';
      }
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
