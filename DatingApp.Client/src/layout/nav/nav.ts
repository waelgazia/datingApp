import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../core/services/account-service';
import { LoginVM } from '../../interfaces/LoginVM';

@Component({
  selector: 'app-nav',
  imports: [ FormsModule ],
  templateUrl: './nav.html',
  styleUrl: './nav.css'
})
export class Nav {
  protected accountService = inject(AccountService);
  protected loginVM = {} as LoginVM;

  login() {
    this.accountService.login(this.loginVM)
      .subscribe({
        next: result => {
          console.log(result);
          this.loginVM = {} as LoginVM;
        },
        error: error => console.error(error)
      });
  }

  loggedOut() {
    this.accountService.logout();
  }
}

