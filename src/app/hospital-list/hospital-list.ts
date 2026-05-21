import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-hospital-list',
  standalone: false,
  templateUrl: './hospital-list.html',
  styleUrl: './hospital-list.scss',
})
export class HospitalList {
@Input() hospitals: any[] = [];
  @Input() isLoading = false;

  getStatusColor(status: string): string {
    switch(status) {
      case 'Full': return 'danger';
      case 'Critical': return 'warning';
      case 'Available': return 'success';
      default: return 'primary';
    }
  }

  getProgressColor(percent: number): string {
    if (percent >= 90) return 'warn';
    if (percent >= 70) return 'accent';
    return 'primary';
  }
}
