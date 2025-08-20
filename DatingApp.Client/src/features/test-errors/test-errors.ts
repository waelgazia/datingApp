import { Component, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-test-errors',
  imports: [],
  templateUrl: './test-errors.html',
  styleUrl: './test-errors.css'
})
export class TestErrors {
  private _httpClient = inject(HttpClient);
  private baseUrl = 'https://localhost:5001/api';
  validationError = signal<string[]>([]);

  get404Error() {
    this._httpClient.get(this.baseUrl + '/buggy/not-found').subscribe({
      next: res => console.log(res),
      error: error => console.log(error)
    });
  }

  get400Error() {
    this._httpClient.get(this.baseUrl + '/buggy/bad-request').subscribe({
      next: res => console.log(res),
      error: error => console.log(error)
    });
  }

  get401Error() {
    this._httpClient.get(this.baseUrl + '/buggy/not-authorized').subscribe({
      next: res => console.log(res),
      error: error => console.log(error)
    });
  }

  get500Error() {
    this._httpClient.get(this.baseUrl + '/buggy/server-error').subscribe({
      next: res => console.log(res),
      error: error => console.log(error)
    });
  }

  get400ValidationError() {
    this._httpClient.post(this.baseUrl + '/accounts/register', {}).subscribe({
      next: res => console.log(res),
      error: error => {
        console.log(error);
        this.validationError.set(error);
      }
    });
  }
}
