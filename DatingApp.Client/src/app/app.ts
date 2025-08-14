import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { Nav } from "../layout/nav/nav";
import { AccountService } from '../core/services/account-service';
import { Home } from "../features/home/home";
import { Member } from '../interfaces/Member';

@Component({
  selector: 'app-root',
  imports: [Nav, Home],
  templateUrl: './app.html',
  styleUrl: './app.css'
})

export class App implements OnInit {
  private _httpClient = inject(HttpClient); /* DI in Angular */
  private _accountService = inject(AccountService);

  protected title : string = 'Dating App';
  protected members = signal<Member[]>([]);

  async ngOnInit() {
    this.setCurrentUser();
    this.members.set(await this.getMembers());
  }

  setCurrentUser() {
    const userString = localStorage.getItem('user');
    if (!userString) return;

    const user = JSON.parse(userString);
    this._accountService.currentUser.set(user);
  }

  async getMembers() {
    try {
      return firstValueFrom(this._httpClient.get<Member[]>("https://localhost:5001/api/members"))
    } catch (error) {
      console.error(error);
      throw error;
    }
  }
}
