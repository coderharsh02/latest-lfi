import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { TestErrorComponent } from './errors/test-error/test-error.component';
import { HomeComponent } from './home/home.component';
import { ListsComponent } from './lists/lists.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { RegisterComponent } from './register/register.component';
import { DonateFormComponent } from './donate-form/donate-form.component';
import { CollectComponent } from './collect/collect.component';
import { DonationDetailComponent } from './donation-detail/donation-detail.component';
import { AuthGuard } from './_guards/auth.guard';
import { RegisterGuard } from './_guards/register.guard';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { PreventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';
import { PreventChangesGuard } from './_guards/prevent-changes.guard';

const routes: Routes = [
  { path: '', component: HomeComponent },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [
      { path: 'members', component: MemberListComponent },
      { path: 'members/:username', component: MemberDetailComponent },
      {
        path: 'member/edit',
        component: MemberEditComponent,
        canDeactivate: [PreventUnsavedChangesGuard],
      },
      {
        path: 'member/changepassword',
        component: ChangePasswordComponent,
        canDeactivate: [PreventChangesGuard],
      },
      { path: 'lists', component: ListsComponent },
      { path: 'donate', component: DonateFormComponent },
      { path: 'collect', component: CollectComponent },
      { path: 'messages', component: MessagesComponent },
      { path: 'donation/:id', component: DonationDetailComponent },
    ],
  },
  {
    path: 'register',
    component: RegisterComponent,
    canActivate: [RegisterGuard],
  },
  { path: 'errors', component: TestErrorComponent },
  { path: 'not-found', component: NotFoundComponent },
  { path: 'server-error', component: ServerErrorComponent },
  { path: '**', component: NotFoundComponent, pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
