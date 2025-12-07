import { Component, ElementRef, inject, ViewChild } from '@angular/core';
import { ConfirmDialogService } from '../../core/services/confirm-dialog-service';

@Component({
  selector: 'app-confirm-dialog',
  imports: [],
  templateUrl: './confirm-dialog.html',
  styleUrl: './confirm-dialog.css'
})
export class ConfirmDialog {
  @ViewChild('confirmDialogRef') confirmDialogRef!: ElementRef<HTMLDialogElement>;

  private resolver: ((result: boolean) => void) | null = null;
  message = 'Are you sure?';

  constructor() {
    inject(ConfirmDialogService).register(this);
  }

  open(message: string) : Promise<boolean> {
    this.message = message;
    this.confirmDialogRef.nativeElement.showModal();

    return new Promise(resolve => (this.resolver = resolve));
  }

  confirm() {
    this.confirmDialogRef.nativeElement.close();
    this.resolver?.(true);
    this.resolver = null;
  }

  cancel() {
    this.confirmDialogRef.nativeElement.close();
    this.resolver?.(false);
    this.resolver = null;
  }
}
