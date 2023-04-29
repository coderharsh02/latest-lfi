import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { DonationsService } from '../_services/donations.service';
import { MatDialog } from '@angular/material/dialog';
import {FeedbackDialogBoxComponent} from '../feedback-dialog-box/feedback-dialog-box.component';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  topDonors: any;
  topCollectors: any;
  question: any;
  feedback: any;

  constructor(
    public accountService: AccountService,
    public dialog: MatDialog,
    private donationService: DonationsService
  ) {}

  ngOnInit(): void {
    this.loadTopDonorsCollectors();
  }

  loadTopDonorsCollectors() {
    this.donationService.getTopDonorCollector().subscribe((response) => {
      this.topDonors = response.topDonors;
      this.topCollectors = response.topCollectors;
    });
  }
}
