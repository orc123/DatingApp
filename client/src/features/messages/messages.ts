import { Component, inject, OnInit, signal } from '@angular/core';
import { MessageService } from '../../core/services/message-service';
import { PaginatedResult } from '../../types/pagination';
import { Message } from '../../types/message';
import { Paginator } from '../../shared/paginator/paginator';
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-messages',
  imports: [Paginator, RouterLink, DatePipe],
  templateUrl: './messages.html',
  styleUrl: './messages.css',
})
export class Messages implements OnInit {
  private messageService = inject(MessageService);
  protected fetchedContainer = 'Inbox';
  protected container = 'Inbox';
  protected pageNumber = 1;
  protected pageSize = 10;
  protected paginateMessages = signal<PaginatedResult<Message> | null>(null);

  tabs = [
    { title: 'Inbox', value: 'Inbox' },
    { title: 'Outbox', value: 'Outbox' },
  ];

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages() {
    this.messageService
      .getMessages(this.container, this.pageNumber, this.pageSize)
      .subscribe({
        next: (response) => {
          this.paginateMessages.set(response);
          this.fetchedContainer = this.container;
        },
        error: (error) => {
          console.error('Error loading messages:', error);
        },
      });
  }

  get IsInbox() {
    return this.fetchedContainer === 'Inbox';
  }

  setContainer(container: string) {
    this.container = container;
    this.pageNumber = 1; // Reset to first page when changing container
    this.loadMessages();
  }
  onPageChange(event: { pageNumber: number; pageSize: number }) {
    this.pageSize = event.pageSize;
    this.pageNumber = event.pageNumber;
    this.loadMessages();
  }

  deleteMessage(event: Event, id: string) {
    event.stopPropagation();
    this.messageService.deleteMessage(id).subscribe({
      next: () => {
        const current = this.paginateMessages();
        if (current?.items) {
          this.paginateMessages.update((prev) => {
            if (!prev) return null;
            const newItems = prev.items.filter((x) => x.id !== id) || [];

            return {
              items: newItems,
              metadata: prev.metadata,
            };
          });
        }
      },
    });
  }
}
