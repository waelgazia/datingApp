import { EMPTY } from 'rxjs';
import { inject } from '@angular/core';
import { ResolveFn, Router } from '@angular/router';

import { Member } from '../../interfaces/models/Member';
import { MemberService } from '../../core/services/member-service';

export const memberResolver: ResolveFn<Member> = (route, state) => {
  const router = inject(Router);

  const memberService = inject(MemberService);
  const memberId = route.paramMap.get('id');

  if (!memberId) {
    router.navigateByUrl('/not-found');
    return EMPTY;
  }

  return memberService.getMember(memberId);
};
