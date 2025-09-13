import { Routes } from '@angular/router';

import { Home } from '../features/home/home';
import { Lists } from '../features/lists/lists';
import { Messages } from '../features/messages/messages';
import { NotFound } from '../shared/errors/not-found/not-found';
import { TestErrors } from '../features/test-errors/test-errors';
import { MemberList } from '../features/members/member-list/member-list';
import { ServerError } from '../shared/errors/server-error/server-error';
import { MemberPhotos } from '../features/members/member-photos/member-photos';
import { MemberProfile } from '../features/members/member-profile/member-profile';
import { MemberDetailed } from '../features/members/member-detailed/member-detailed';
import { MemberMessages } from '../features/members/member-messages/member-messages';

import { memberResolver } from '../features/members/member-resolver';
import { authenticateGuard } from '../core/guards/authenticate-guard';

export const routes: Routes = [
  { path: '', component: Home },
  { // workaround to use a single guard on punch of routes.
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [authenticateGuard],
    children: [
      { path: 'members', component: MemberList },
      {
        path: 'members/:id',
        resolve: { member: memberResolver },
        runGuardsAndResolvers: 'always',
        component: MemberDetailed,
        children: [
          { path: '', redirectTo: 'profile', pathMatch: 'full' },  /* redirect members/:id to profile */
          { path: 'profile', component: MemberProfile, title: 'Profile' },
          { path: 'photos', component: MemberPhotos, title: 'Photos' },
          { path: 'messages', component: MemberMessages, title: 'Messages' }
        ]
      },
      { path: 'lists', component: Lists },
      { path: 'messages', component: Messages },
    ]
  },
  { path: 'errors', component: TestErrors },
  { path: 'server-error', component: ServerError },
  { path: '**', component: NotFound }
];
