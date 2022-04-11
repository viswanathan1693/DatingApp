import { ConfirmService } from './../_services/confirm.service';
import { MemberEditComponent } from './../members/member-edit/member-edit.component';
import { Injectable, Component } from '@angular/core';
import { CanDeactivate, ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<unknown> {
  
  constructor(private confirmService: ConfirmService) {}
  
  canDeactivate(Component: MemberEditComponent): Observable<boolean> | boolean {
    if(Component.editForm.dirty){
      return this.confirmService.confirm();
    }
    return true;
  }

}
