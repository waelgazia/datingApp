import { FormsModule } from '@angular/forms';
import { Component, inject, output } from '@angular/core';

import { RegisterVM } from '../../../interfaces/models/RegisterVM';
import { AccountService } from '../../../core/services/account-service';

@Component({
  selector: 'app-register',
  imports: [ FormsModule ],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {
  private _accountService = inject(AccountService);

  protected registerVM = {} as RegisterVM; /* give a type without initialize the values */
  cancelRegister = output<boolean>();

  register() {
    this._accountService.register(this.registerVM)
      .subscribe({
        next: result => {
          console.log(result)
          this.cancel();
        },
        error: error => console.error(error)
      })
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
