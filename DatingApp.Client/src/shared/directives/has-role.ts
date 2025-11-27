import { Directive, inject, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';

import { AccountService } from '../../core/services/account-service';

@Directive({
  selector: '[appHasRole]'
})
export class HasRole implements OnInit{
  @Input() appHasRole: string[] = [];

  private _accountService = inject(AccountService);
  private _viewContainerRef = inject(ViewContainerRef);
  private _templateRef = inject(TemplateRef);

  ngOnInit(): void {
    // this._viewContainerRef.createEmbeddedView(this._templateRef) is the core operation that a
    // structural directive uses to insert the template into the DOM. createEmbeddedView() Injects
    // the template into the DOM

    if (this._accountService.currentUser()?.roles.some(r => this.appHasRole.includes(r))) {
      this._viewContainerRef.createEmbeddedView(this._templateRef);
    } else {
      this._viewContainerRef.clear();
    }
  }
}


/*
  To use this directive in a template:
  <a *appHasRole="['Admin', 'Moderator']" routerLink="/admin" routerLinkActive="text-accent">Admin</a>
*/
