import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Donation } from '../_models/donation';

@Injectable({
  providedIn: 'root'
})
export class DonationsService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getDonation(id: number) {
    return this.http.get<Donation>(this.baseUrl + 'donations/id/' + id);
  }

  getDonations() {
    return this.http.get<Donation[]>(this.baseUrl + 'donations');
  }

  donateNow(donation: Donation) {
    return this.http.post<any>(this.baseUrl + 'donations', donation);
  }

  addCollector(donation: any) {
    return this.http.put<any>(this.baseUrl + 'donations/addCollector', donation);
  }

  getTopDonorCollector() {
    return this.http.get<any>(this.baseUrl + 'donations/top');
  }

  updateToCollected(donation: any) {
    return this.http.put<any>(this.baseUrl + 'donations/updateDonationStatusToCollected', donation);
  }

  updateToDonated(donation: any) {
    return this.http.put<any>(this.baseUrl + 'donations/updateDonationStatusToDonated', donation);
  }
}
