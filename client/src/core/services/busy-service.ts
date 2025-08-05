import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class BusyService {
  busytRequestCount = signal(0);
  busy() {
    this.busytRequestCount.update((current) => current + 1);
  }

  idle() {
    this.busytRequestCount.update((current) => Math.max(0, current - 1));
  }
}
