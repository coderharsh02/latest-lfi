import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';
import { DonationsService } from '../_services/donations.service';
import { MembersService } from '../_services/members.service';

@Component({
  selector: 'app-donate-form',
  templateUrl: './donate-form.component.html',
  styleUrls: ['./donate-form.component.css'],
})
export class DonateFormComponent implements OnInit {
  member: any;
  user: User | undefined | null;

  constructor(
    private accountServices: AccountService,
    private memberService: MembersService,
    private donationService: DonationsService,
    private router: Router,
    private toastr: ToastrService
  ) {
    this.accountServices.currentUser$
      .pipe(take(1))
      .subscribe((user) => (this.user = user));
  }

  ngOnInit(): void {
    this.loadMember();
  }

  model: any = {};

  loadMember() {
    this.memberService.getMember(this.user?.username).subscribe((member) => {
      this.member = member;
    });
  }

  donateNow() {
    this.model.donorId = this.member.user.userId;
    this.donationService.donateNow(this.model).subscribe({
      next: () => {
        this.router.navigateByUrl('/collect');
      },
      error: error => {
        this.toastr.error(error.error);
        console.log(error);
      }
    });
  }
}
