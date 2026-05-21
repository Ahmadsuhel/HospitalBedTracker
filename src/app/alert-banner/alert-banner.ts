import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-alert-banner',
  standalone: false,
  templateUrl: './alert-banner.html',
  styleUrl: './alert-banner.scss',
})
export class AlertBanner {
  @Input() alerts: any[] = [];
}
