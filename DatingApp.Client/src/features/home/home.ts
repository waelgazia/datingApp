import { Component, Input, signal } from '@angular/core';
import { Register } from "../account/register/register";
import { Member } from '../../interfaces/Member';

@Component({
  selector: 'app-home',
  imports: [Register],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home {
  protected registerMode = signal<boolean>(false);

  showRegister(value: boolean) {
    this.registerMode.set(value);
  }
}
