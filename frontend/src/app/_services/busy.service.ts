import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class BusyService {
  busyReuqestCount = 0;

  constructor(private spinnerService: NgxSpinnerService) { }

  busy() {
    this.busyReuqestCount++;
    this.spinnerService.show(undefined, {
      type: 'line-scale-party',
      bdColor: 'rgba(255,255,255,0)',
      color: '#1a8754'
    });
  }

  idle() {
    this.busyReuqestCount--;
    if (this.busyReuqestCount <= 0) {
      this.busyReuqestCount = 0;
      this.spinnerService.hide();
    }
  }
}
