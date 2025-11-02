import { EMPTY } from 'rxjs';
import { inject } from '@angular/core';
import { ResolveFn, Router } from '@angular/router';

import { MemberDto } from '../../interfaces/models/MemberDto';
import { MemberService } from '../../core/services/member-service';

export const memberResolver: ResolveFn<MemberDto> = (route, state) => {
  const router = inject(Router);

  const memberService = inject(MemberService);
  const memberId = route.paramMap.get('id');

  if (!memberId) {
    router.navigateByUrl('/not-found');
    return EMPTY;
  }

  return memberService.getMember(memberId);
};
