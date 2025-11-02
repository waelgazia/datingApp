import { RouterLink } from '@angular/router';
import { Component, input } from '@angular/core';

import { AgePipe } from '../../../core/pipes/age-pipe';
import { MemberDto } from '../../../interfaces/models/MemberDto';

@Component({
  selector: 'app-member-card',
  imports: [RouterLink, AgePipe],
  templateUrl: './member-card.html',
  styleUrl: './member-card.css'
})
export class MemberCard {
  member = input.required<MemberDto>();
}
