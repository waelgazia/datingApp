import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  private _toastContainerId = 'toast-container';


  constructor() {
    this.createToastContainer();
  }

  private createToastContainer () {
    if (!document.getElementById(this._toastContainerId)) {
      const container = document.createElement('div');
      container.id = this._toastContainerId;
      container.className = 'toast toast-bottom toast-end';

      document.body.appendChild(container);
    }
  }

  private createToastElement(message: string, alertClass: string, duration: number = 5000) {
    const toastContainer = document.getElementById(this._toastContainerId);
    if (!toastContainer) return;

    const toast = document.createElement('div');
    toast.classList.add('alert', alertClass, 'shadow-lg');
    toast.innerHTML = `
      <span>${message}</span>
      <button class="ml-4 btn btn-sm btn-ghost">x</button>
    `;

    toast.querySelector('button')?.addEventListener('click', () => {
      toastContainer.removeChild(toast);
    });

    toastContainer.append(toast);
    setTimeout(() => {
      if (toastContainer.contains(toast)) {
        toastContainer.removeChild(toast);
      }
    }, duration);
  }

  success(message: string, duration?: number) : void {
    this.createToastElement(message, 'alert-success', duration);
  }

  error(message: string, duration?: number) : void {
    this.createToastElement(message, 'alert-error', duration);
  }

  warning(message: string, duration?: number) : void {
    this.createToastElement(message, 'alert-warning', duration);
  }

  info(message: string, duration?: number) : void {
    this.createToastElement(message, 'alert-info', duration);
  }
}
