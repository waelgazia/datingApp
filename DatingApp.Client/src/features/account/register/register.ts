import { Router } from '@angular/router';
import { Component, inject, output, signal } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';

import { TextInput } from "../../../shared/text-input/text-input";
import { AccountService } from '../../../core/services/account-service';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, TextInput],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {
  private _accountService = inject(AccountService);
  private _formBuilderService = inject(FormBuilder); /* angular service to build form */
  private _router = inject(Router);

  public cancelRegister = output<boolean>();
  protected credentialForm: FormGroup;
  protected profileForm: FormGroup;
  protected currentStep = signal<number>(1);
  protected validationErrors = signal<string[]>([])

  constructor() {
    this.credentialForm = this._formBuilderService.group({
      email: ['', [Validators.required, Validators.email]],
      displayName: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]],
    });

    // subscribe to the password control changes to ensure validity with confirmPassword
    this.credentialForm.controls['password'].valueChanges.subscribe(() => {
      this.credentialForm.controls['confirmPassword'].updateValueAndValidity();
    });

    this.profileForm = this._formBuilderService.group({
      gender: ['male', Validators.required],
      dateOfBirth: ['', [Validators.required, this.olderThanEighteen()]],
      city: ['', Validators.required],
      country: ['', Validators.required]
    });
  }

  matchValues(controlToMatch: string): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const form = control.parent;   /* get control parent which is the form */
      if (!form) return null;

      // control the password control value with the confirm password value to ensure the equality
      const matchValue = form.get(controlToMatch)?.value;
      return control.value === matchValue ? null : { passwordMismatch: true }
    }
  }

  olderThanEighteen(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (control.value === '') return null;

      const userDateOfBirth = new Date(control.value);
      const userAge = (new Date()).getFullYear() - userDateOfBirth.getFullYear();

      return userAge >= 18 ? null : { lessThanEighteen: true };
    }
  }

  nextStep() {
    if (this.credentialForm.valid) {
      this.currentStep.update(currentStep => currentStep + 1);
    }
  }

  previousStep() {
    this.currentStep.update(currentStep => currentStep - 1);
  }

  register() {
    if (this.credentialForm.valid && this.profileForm.valid) {
      const formData = { ...this.credentialForm.value, ...this.profileForm.value }
      this._accountService.register(formData)
        .subscribe({
          next: () => {
            this._router.navigateByUrl('/members')
          },
          error: error => {
            console.error(error)
            this.validationErrors.set(error);
          }
        })
    }
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
