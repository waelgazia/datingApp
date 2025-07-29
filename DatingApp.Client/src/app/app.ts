import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-root',
  imports: [],
  templateUrl: './app.html',
  styleUrl: './app.css'
})

export class App implements OnInit {
  private httpClient = inject(HttpClient); /* DI in Angular */

  protected title = 'Dating App';
  /*
    As we are using zoneless approach, angular won't know when the prop value changes so,
    instead of using protected members : any, we need to use signal
  */
 protected members = signal<any>([]);

  async ngOnInit() {
    /*
      Using subscribe is not a good idea, because if something happened in the system and the
      call is not completed or had an error it won't be dispose and will continue listening
      this.httpClient.get("https://localhost:5001/api/members")
        .subscribe({
          next: response => this.members.set(response),
          error: err => console.error(err),
          complete: () => console.log("Completed the http request!")
        })
    */

    this.members.set(await this.getMembers());
  }

  async getMembers() {
    try {
      /*
        use firstValueFrom or lastValueFrom instead of subscribe as we won't expect
        a stream (no need to use await as we want to return a promise)
      */
      return firstValueFrom(this.httpClient.get("https://localhost:5001/api/members"))
    } catch (error) {
      console.error(error);
      throw error;
    }
  }
}
