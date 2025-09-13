import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Component, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';

import { AccountService } from '../../core/services/account-service';
import { LoginVM } from '../../interfaces/LoginVM';
import { ToastService } from '../../core/services/toast-service';
import { AccountService } from '../../core/services/account-service';

@Component({
  selector: 'app-nav',
  imports: [ FormsModule, RouterLink, RouterLinkActive ],
  templateUrl: './nav.html',
  styleUrl: './nav.css'
})
export class Nav {
  private _router = inject(Router)
  private _toastService = inject(ToastService);

  protected accountService = inject(AccountService);
  protected loginVM = {} as LoginVM;

  login() {
    this.accountService.login(this.loginVM)
      .subscribe({
        next: () => {
          this.loginVM = {} as LoginVM;
          this._router.navigateByUrl('/members');
          this._toastService.success('Logged in successfully!');
        },
        error: error => {
          console.error(error);
          this._toastService.error('Error happens when signing in');
        }
      });
  }

  loggedOut() {
    this.accountService.logout();
    this._router.navigateByUrl('/');
  }
}

