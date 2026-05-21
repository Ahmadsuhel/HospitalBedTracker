import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-dashboard',
  standalone: false,
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard {
@Input() cityData: any;
  @Input() isLoading = true;
  @Input() lastUpdated: Date = new Date();
}
