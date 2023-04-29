import { Component, HostListener, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css'],
})
export class MemberEditComponent {
  @ViewChild('editForm') editForm: NgForm | any;

  @HostListener('window:beforeunload', ['$event']) unloadNotification(
    $event: any
  ) {
    if (this.editForm?.dirty) {
      $event.returnValue = true;
    }
  }

  member: any;
  user: User | null = null;

  constructor(
    private accountServices: AccountService,
    private memberService: MembersService,
    private toastr: ToastrService
  ) {
    this.accountServices.currentUser$
      .pipe(take(1))
      .subscribe((user) => (this.user = user));
  }

  ngOnInit(): void {
    this.loadMember();
  }
  
  loadMember() {
    if (!this.user) return;

    this.memberService.getMember(this.user.username).subscribe({
      next: (member) => {
        this.member = member;
        console.log(this.member);
      },
    });
  }

  updateMember() {
    console.log(this.member.user);
    this.memberService.updateMember(this.member.user).subscribe({
      next: () => {
        this.toastr.success('Profile updated successfully');
        this.editForm?.reset(this.member.user);
      },
      error: (err) => {
        this.toastr.error(err.error);
      },
    });
  }
}
