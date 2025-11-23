import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Component, inject, OnInit, signal } from '@angular/core';

import { Paginator } from '../../shared/paginator/paginator';
import { MessageDto } from '../../interfaces/models/MessageDto';
import { MessagesService } from '../../core/services/messages-service';
import { PaginatedResult } from '../../interfaces/base/PaginatedResult';
import { MESSAGES_CONTAINERS } from '../../constants/messages-containers';
import { MessagesParameters } from '../../interfaces/ResourceParameters/MessagesParameters';

@Component({
  selector: 'app-messages',
  imports: [Paginator, RouterLink, DatePipe],
  templateUrl: './messages.html',
  styleUrl: './messages.css'
})
export class Messages implements OnInit {
  private _messagesService = inject(MessagesService);

  // used to update UI after fetching the messages based on the container
  protected fetchedContainer = MESSAGES_CONTAINERS.INBOX;
  protected messagesParameters = new MessagesParameters();
  protected paginatedMessages = signal<PaginatedResult<MessageDto> | null>(null);
  tabs = [
    { label: 'Inbox', value: MESSAGES_CONTAINERS.INBOX },
    { label: 'Outbox', value: MESSAGES_CONTAINERS.OUTBOX }
  ];

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages() {
    this._messagesService.getMessages(this.messagesParameters).subscribe({
      next: result => {
        this.paginatedMessages.set(result)
        this.fetchedContainer = this.messagesParameters.container;
      }
    });
  }

  deleteMessage(event: Event, messageId: string) {
    event.stopPropagation();

    this._messagesService.deleteMessage(messageId).subscribe({
      next: () => {
        // you can just call this.loadMessages() instead of doing the following
        // (you will need to update the caching in loading-service first)

        const currentMessages = this.paginatedMessages();
        if (currentMessages?.items) {
          this.paginatedMessages.update(previousMessages => {
            if (!previousMessages) return null;

            const updatedMessages = previousMessages.items.filter(m => m.id !== messageId) || [];
            return {
              items: updatedMessages,
              paginationMetadata: previousMessages.paginationMetadata
            }
          })
        }
      }
    })
  }

  get isInbox() {
    return this.fetchedContainer === MESSAGES_CONTAINERS.INBOX;
  }

  setContainer(container: string) {
    this.messagesParameters = new MessagesParameters();
    this.messagesParameters.container = container;
    this.loadMessages();
  }

  onPageChanged(event: { pageNumber: number, pageSize: number }) {
    this.messagesParameters.pageNumber = event.pageNumber;
    this.messagesParameters.pageSize = event.pageSize;
    this.loadMessages();
  }
}