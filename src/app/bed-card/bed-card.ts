import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-bed-card',
  standalone: false,
  templateUrl: './bed-card.html',
  styleUrl: './bed-card.scss',
})
export class BedCard {
 @Input() title = '';
  @Input() icon = '';
  @Input() available = 0;
  @Input() total = 0;
  @Input() color = 'primary';

  get occupancyPercent(): number {
    if (!this.total) return 0;
    return Math.round(((this.total - this.available) / this.total) * 100);
  }

  get statusLabel(): string {
    if (this.occupancyPercent >= 90) return 'Critical';
    if (this.occupancyPercent >= 70) return 'High';
    return 'Available';
  }

  get statusClass(): string {
    if (this.occupancyPercent >= 90) return 'status-critical';
    if (this.occupancyPercent >= 70) return 'status-warning';
    return 'status-ok';
  }
}
