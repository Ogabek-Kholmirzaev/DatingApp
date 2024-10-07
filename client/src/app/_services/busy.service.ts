import { inject, Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class BusyService {
  baseRequestCount = 0;
  private spinnerService = inject(NgxSpinnerService);

  busy() {
    this.baseRequestCount++;
    this.spinnerService.show(undefined, {
      type: 'line-scale-party',
      bdColor: 'rgba(255,255,255,0)',
      color: '#333333'
    });
  }

  idle() {
    this.baseRequestCount--;

    if (this.baseRequestCount <= 0) {
      this.baseRequestCount = 0;
      this.spinnerService.hide();
    }
  }

  constructor() { }
}
