import { Routes } from '@angular/router';

import { Home } from '../features/home/home';
import { Lists } from '../features/lists/lists';
import { Messages } from '../features/messages/messages';
import { MemberList } from '../features/members/member-list/member-list';
import { MemberDetailed } from '../features/members/member-detailed/member-detailed';

import { authenticateGuard } from '../core/guards/authenticate-guard';

export const routes: Routes = [
  { path: '', component: Home },
  { // workaround to use a single guard on punch of routes.
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [authenticateGuard],
    children: [
      { path: 'members', component: MemberList },
      { path: 'members/:id', component: MemberDetailed },
      { path: 'lists', component: Lists },
      { path: 'messages', component: Messages },
    ]
  },
  { path: '**', component: Home }
];
