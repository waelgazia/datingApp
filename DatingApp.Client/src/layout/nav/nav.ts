import { FormsModule } from '@angular/forms';
import { Component, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';

import { themes } from '../theme';
import { STORAGE_KEY } from '../../constants/storage-keys';
import { LoginDto } from '../../interfaces/models/LoginDto';
import { BusyService } from '../../core/services/busy-service';
import { ToastService } from '../../core/services/toast-service';
import { AccountService } from '../../core/services/account-service';

@Component({
  selector: 'app-nav',
  imports: [ FormsModule, RouterLink, RouterLinkActive ],
  templateUrl: './nav.html',
  styleUrl: './nav.css'
})
export class Nav implements OnInit{
  private _router = inject(Router)
  private _toastService = inject(ToastService);

  protected busyService = inject(BusyService);
  protected accountService = inject(AccountService);
  protected loginDto = {} as LoginDto;

  protected selectedTheme = signal<string>(localStorage.getItem(STORAGE_KEY.THEME) || 'light');
  protected themes : string[] = themes;

  ngOnInit(): void {
    // to set a theme using daisyUI
    document.documentElement.setAttribute('data-theme', this.selectedTheme());
  }

  login() {
    this.accountService.login(this.loginDto)
      .subscribe({
        next: () => {
          this.loginDto = {} as LoginDto;
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
    this.accountService.logout().subscribe({
      next: () => this._router.navigateByUrl('/')
    });
  }

  onThemeSelected(theme: string) {
    this.selectedTheme.set(theme);
    localStorage.setItem(STORAGE_KEY.THEME, theme);

    // to set a theme using daisyUI
    document.documentElement.setAttribute('data-theme', theme);

    // to automatically close the dropdown after theme selection
    const themeDropdown = document.activeElement as HTMLDivElement;
    if (themeDropdown) {
      themeDropdown.blur();
    }
  }
}

