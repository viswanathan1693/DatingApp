import { MemberEditComponent } from './../members/member-edit/member-edit.component';
import { Injectable, Component } from '@angular/core';
import { CanDeactivate, ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<unknown> {
  canDeactivate(Component: MemberEditComponent): boolean {
    if(Component.editForm.dirty){
      return confirm('Are you sure you want to continue ? Any unsaved changes will be lost')
    }
    return true;
  }

}
