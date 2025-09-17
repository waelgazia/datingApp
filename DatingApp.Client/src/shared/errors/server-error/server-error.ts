import { Router } from '@angular/router';
import { Location } from '@angular/common';
import { Component, inject, signal } from '@angular/core';

import { ApiError } from '../../../interfaces/base/ApiError';

@Component({
  selector: 'app-server-error',
  imports: [],
  templateUrl: './server-error.html',
  styleUrl: './server-error.css'
})
export class ServerError {
  private _location = inject(Location);
  protected error : ApiError;
  private _router = inject(Router);
  protected showDetails = signal<boolean>(false);

  constructor() {
    const navigation = this._router.getCurrentNavigation();
    this.error = navigation?.extras?.state?.['error'];
  }

  detailsToggle() {
    this.showDetails.set(!this.showDetails());
  }

  goBack() {
    this._location.back();
  }
}
