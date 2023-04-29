import { Component, HostListener, ViewChild } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css'],
})
export class ChangePasswordComponent {
  @ViewChild('changeForm') changeForm: NgForm | any;

  @HostListener('window:beforeunload', ['$event']) unloadNotification(
    $event: any
  ) {
    if (this.changeForm?.dirty) {
      $event.returnValue = true;
    }
  }

  model: any = {};

  constructor(
    public accountService: AccountService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.accountService.currentUser$.subscribe((user) => {
      this.model.username = user?.username;
    });
  }
  changePassword() {
    this.accountService.changePassword(this.model).subscribe({
      next: () => {
        this.toastr.success('Password changed successfully!');
        this.changeForm?.reset(this.model);
        this.router.navigateByUrl('/');
      },
      error: (error) => {
        this.toastr.error(error.error);
        console.log(error);
      },
    });
  }
}
