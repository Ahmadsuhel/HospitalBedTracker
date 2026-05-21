import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { BedApi } from './services/bed-api';

import * as signalR from '@microsoft/signalr'
import { SignalrService } from './services/signalr';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  standalone: false,
    styleUrls: ['./app.scss']
})
export class App implements OnInit, OnDestroy {
  selectedCity = 'Delhi';
  cityData: any = null;
  isLoading = true;
  lastUpdated = new Date();
  criticalAlerts: any[] = [];
  private destroy$ = new Subject<void>();

  constructor(
    private bedApi: BedApi,
    private signal: SignalrService
  ) {}

  ngOnInit() {
    this.loadDashboard();
    this.connectSignalR();
  }

  loadDashboard() {
    this.isLoading = true;
    this.bedApi.getCityDashboard(this.selectedCity)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data:any) => {
          this.cityData = data;
          this.isLoading = false;
          this.lastUpdated = new Date();
        },
        error: () => { this.isLoading = false; }
      });
  }

  connectSignalR() {
    this.signal.startConnection();

    this.signal.joinCityGroup(this.selectedCity);

    this.signal.bedUpdated$
      .pipe(takeUntil(this.destroy$))
      .subscribe((update: any) => {
        if (update) {
          this.loadDashboard();
          this.lastUpdated = new Date();
        }
      });

    this.signal.criticalAlert$
      .pipe(takeUntil(this.destroy$))
      .subscribe((alert: any) => {
        if (alert) {
          this.criticalAlerts = [alert, ...this.criticalAlerts].slice(0, 5);
        }
      });
  }

  onCityChange(event: any) {
    this.selectedCity = event.value;
    this.signal.joinCityGroup(this.selectedCity);
    this.loadDashboard();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
