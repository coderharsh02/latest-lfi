import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DonationsService } from 'src/app/_services/donations.service';

@Component({
  selector: 'app-donation-detail',
  templateUrl: './donation-detail.component.html',
  styleUrls: ['./donation-detail.component.css'],
})

export class DonationDetailComponent implements OnInit {
  donation: any;
  isAvailable: boolean = false;
  donationStr: any;

  constructor(
    private donationService: DonationsService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.loadDonation();
  }

  loadDonation() {
    var donationId = Number(this.route.snapshot.paramMap.get('id'));
    if (!donationId) return;
    this.donationService.getDonation(donationId).subscribe({
      next: (donation) => {
        this.donation = donation;
        this.isAvailable = this.donation.status == 'Available';
        console.log(this.donation);
        this.donationStr = JSON.stringify(this.donation);
      },
      error: (error) => {
        console.log(error);
      }
    });
  }
}
