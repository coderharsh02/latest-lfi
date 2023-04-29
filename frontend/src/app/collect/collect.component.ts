import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { DonationsService } from '../_services/donations.service';
import { Columns, Config, DefaultConfig } from 'ngx-easy-table';
import { AccountService } from '../_services/account.service';
import { MembersService } from '../_services/members.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { FeedbackDialogBoxComponent } from '../feedback-dialog-box/feedback-dialog-box.component';
import { ConfirmationDialog } from '../confirm/confirm.component';

@Component({
  selector: 'app-collect',
  templateUrl: './collect.component.html',
  styleUrls: ['./collect.component.css'],
})
export class CollectComponent {
  user: any;
  member: any;
  displayedColumns: string[] = [];
  dataSource: MatTableDataSource<any> = new MatTableDataSource<any>([]);

  @ViewChild(MatPaginator) paginator: any = MatPaginator;
  @ViewChild(MatSort) sort: any = MatSort;

  constructor(
    private accountServices: AccountService,
    private memberService: MembersService,
    private donationService: DonationsService,
    private toastr: ToastrService,
    public dialog: MatDialog
  ) {
    this.loadMember();
    this.loadTable();
  }

  ngOnInit(): void {}

  ngAfterViewInit() {}

  loadMember() {
    this.accountServices.currentUser$
      .pipe(take(1))
      .subscribe((user) => (this.user = user));

    if (!this.user) return;
    this.memberService.getMember(this.user.username).subscribe({
      next: (member) => {
        this.member = member;
        console.log(this.member);
      },
    });
  }

  loadTable() {
    this.displayedColumns = [
      'donar-name',
      'no-of-meals',
      'donor-pincode',
      'status',
    ];

    this.donationService.getDonations().subscribe({
      next: (donations) => {
        this.dataSource = new MatTableDataSource(
          donations.map((donation: any) => {
            console.log(donation);
            return {
              id: donation.donationId,
              noOfMeals: donation.noOfMeals,
              status: donation.status,
              feedbackByDonor: 'Collector coming to collect',
              feedbackByCollector: 'On the way to collect food',
              donorId: donation.donatedBy.userId,
              donorName: donation.donatedBy.name,
              pincode: donation.donatedBy.pincode,
            };
          })
        );
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        this.dataSource.filter = 'Available';
        console.log(donations);
      },
    });
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  eventEmitted(donation: any): void {
    const confirmRef = this.dialog.open(ConfirmationDialog, {
      data: {
        message: 'Are you sure you want to collect this donation?',
      },
    });
    confirmRef.afterClosed().subscribe((result) => {
      if (result) {
        if (donation.status !== 'Available') {
          this.toastr.error('This donation is already collected!');
        } else {
          var feedback = 'I am on my way to collect the food';
          const dialogRef = this.dialog.open(FeedbackDialogBoxComponent, {
            data: {
              question: 'Please enter some message for donor..',
              answer: feedback,
            },
          });
          dialogRef.afterClosed().subscribe((result) => {
            feedback = result;
            var updatedDonation = {
              id: donation.id,
              noOfMeals: donation.noOfMeals,
              status: donation.status,
              feedbackByDonor:
                '<p><strong>Status(Available): </strong>Collector coming to collect the food</p>',
              feedbackByCollector:
                '<p><strong>Status(Available): </strong>' + feedback + '</p>',
              donorId: donation.donorId,
              collectorId: this.member?.user.userId,
            };

            this.donationService.addCollector(updatedDonation).subscribe({
              next: () => {
                this.toastr.success('Status Updated Successfully!');
                this.loadTable();
              },
              error: (error) => {
                this.toastr.error(error.error);
                this.loadTable();
                console.log(error);
              },
            });
          });
        }
      }
    });
  }
}
