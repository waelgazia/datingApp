import { Component, input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  imports: [ReactiveFormsModule],
  templateUrl: './text-input.html',
  styleUrl: './text-input.css'
})

export class TextInput implements ControlValueAccessor {
  public label = input.required<string>();
  public type = input<string>('text');

  // @Self() is an Angular dependency injection modifier. that tells Angular to
  // inject the NgControl from this component’s own injector, not from any parent.
  constructor(@Self() public ngControl: NgControl) {
    this.ngControl.valueAccessor = this;
  }

  writeValue(obj: any): void { }
  registerOnChange(fn: any): void { }
  registerOnTouched(fn: any): void { }

  get control() : FormControl {
    return this.ngControl.control as FormControl;
  }
}
