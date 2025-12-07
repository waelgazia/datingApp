import { Injectable } from '@angular/core';

import { ConfirmDialog } from '../../shared/confirm-dialog/confirm-dialog';

@Injectable({
  providedIn: 'root'
})
export class ConfirmDialogService {
  private _confirmDialogComponent?: ConfirmDialog;

  register(confirmDialogComponent: ConfirmDialog) {
    this._confirmDialogComponent = confirmDialogComponent;
  }

  confirm(message: string = 'Are you sure?') : Promise<boolean> {
    if (!this._confirmDialogComponent) {
      throw new Error('Confirm dialog component is not registered!');
    }

    return this._confirmDialogComponent.open(message);
  }
}