import { Component, inject } from '@angular/core';
import { Location } from '@angular/common'

@Component({
  selector: 'app-not-found',
  imports: [],
  templateUrl: './not-found.html',
  styleUrl: './not-found.css'
})
export class NotFound {
  private _location = inject(Location);   /* A service allows interacting with a browser's URL. */

  goBack() {
    this._location.back(); // go back to the previous url
  }
}
