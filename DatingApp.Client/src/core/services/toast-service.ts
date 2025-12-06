import { Router } from '@angular/router';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  private _toastContainerId = 'toast-container';
  private _router = inject(Router);

  constructor() {
    this.createToastContainer();
  }

  private createToastContainer () {
    if (!document.getElementById(this._toastContainerId)) {
      const container = document.createElement('div');
      container.id = this._toastContainerId;
      container.className = 'toast toast-bottom toast-end z-50';

      document.body.appendChild(container);
    }
  }

  private createToastElement(message: string, alertClass: string, duration: number = 5000, avatar?: string, route?: string) {
    const toastContainer = document.getElementById(this._toastContainerId);
    if (!toastContainer) return;

    const toast = document.createElement('div');
    toast.classList.add('alert', alertClass, 'shadow-lg', 'flex', 'items-center', 'gap-3', 'cursor-pointer');

    if (route) {
      toast.addEventListener('click', () => {
        this._router.navigateByUrl(route);
        setTimeout(() => toastContainer.removeChild(toast), 500);
      });
    }

    toast.innerHTML = `
      ${avatar ? `<img src=${avatar || './user.png'} class='w-10 h-10 rounded'`: ''}
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

  success(message: string, duration?: number, avatar?: string, route?: string) : void {
    this.createToastElement(message, 'alert-success', duration, avatar, route);
  }

  error(message: string, duration?: number, avatar?: string, route?: string) : void {
    this.createToastElement(message, 'alert-error', duration, avatar, route);
  }

  warning(message: string, duration?: number, avatar?: string, route?: string) : void {
    this.createToastElement(message, 'alert-warning', duration, avatar, route);
  }

  info(message: string, duration?: number, avatar?: string, route?: string) : void {
    this.createToastElement(message, 'alert-info', duration, avatar, route);
  }
}
