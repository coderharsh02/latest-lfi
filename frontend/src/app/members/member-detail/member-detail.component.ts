import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { UserDetail } from 'src/app/_models/userDetail';
import { MembersService } from 'src/app/_services/members.service';
import { DonationsService } from 'src/app/_services/donations.service';
import { ToastrService } from 'ngx-toastr';
import { MatDialog } from '@angular/material/dialog';
import { FeedbackDialogBoxComponent } from '../../feedback-dialog-box/feedback-dialog-box.component';
import { ConfirmationDialog } from '../../confirm/confirm.component';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
})
export class MemberDetailComponent implements OnInit {
  member: any;
  question: any;

  constructor(
    private memberService: MembersService,
    private route: ActivatedRoute,
    private toastr: ToastrService,
    private donationService: DonationsService,
    public dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    var username = this.route.snapshot.paramMap.get('username');
    if (!username) return;
    this.memberService.getMember(username).subscribe({
      next: (member) => {
        this.member = member;
        console.log(this.member);
      },
    });
  }

  eventEmitted(donation: any): void {
    if (donation.status === 'Donated') {
      this.toastr.error('This donation is already donated!');
    } else if (donation.status === 'Booked') {
      const confirmRef = this.dialog.open(ConfirmationDialog, {
        data: {
          message: 'Are you sure you have collected this donation?',
        },
      });
      confirmRef.afterClosed().subscribe((result) => {
        if (result) {
          var updatedDonation = {
            id: donation.donationId,
            noOfMeals: donation.noOfMeals,
            status: donation.status,
            feedbackByDonor:
              donation.feedbackByDonor +
              '<p><strong>Status(Collected): </strong>Collector collected the food</p>',
            feedbackByCollector:
              donation.feedbackByCollector +
              '<p><strong>Status(Collected): </strong>On the way to donate the food</p>',
            donorId: donation.donorId,
            collectorId: this.member?.user.userId,
          };
          this.donationService.updateToCollected(updatedDonation).subscribe({
            next: () => {
              this.toastr.success('Collected Successfully!');
              this.loadMember();
            },
            error: (error) => {
              this.toastr.error(error.error);
              this.loadMember();
              console.log(error);
            },
          });
        }
      });
    } else if (donation.status === 'Collected') {
      const confirmRef = this.dialog.open(ConfirmationDialog, {
        data: {
          message: 'Are you sure you have donated this donation?',
        },
      });
      confirmRef.afterClosed().subscribe((result) => {
        if (result) {
          var feedback = 'Food donated to the needy!';
          const dialogRef = this.dialog.open(FeedbackDialogBoxComponent, {
            data: {
              question: 'Please enter some message about donation..',
              answer: feedback,
            },
          });

          dialogRef.afterClosed().subscribe((result) => {
            feedback = result;
            var updatedDonation = {
              id: donation.donationId,
              noOfMeals: donation.noOfMeals,
              status: donation.status,
              feedbackByDonor:
                donation.feedbackByDonor +
                '<p><strong>Status(Available): </strong>Collector coming to collect the food</p>',
              feedbackByCollector:
                donation.feedbackByCollector +
                '<p><strong>Status(Available): </strong>' +
                feedback +
                '</p>',
              donorId: donation.donorId,
              collectorId: this.member?.user.userId,
            };

            this.donationService.updateToDonated(updatedDonation).subscribe({
              next: () => {
                this.toastr.success('Donated Successfully!');
                this.loadMember();
              },
              error: (error) => {
                this.toastr.error(error.error);
                this.loadMember();
                console.log(error);
              },
            });
          });
        }
      });
    }
  }
}
