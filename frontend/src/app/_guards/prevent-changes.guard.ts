import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { ChangePasswordComponent } from '../change-password/change-password.component';

@Injectable({
  providedIn: 'root'
})
export class PreventChangesGuard implements CanDeactivate<unknown> {
  canDeactivate(
    component: ChangePasswordComponent): boolean  {
    if(component.changeForm.dirty)
    {
      return confirm('Are you sure you want to continue? Any unsaved changes will be lost');
    }
    return true;
  }
}
