import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class BedApi {
   private apiUrl = 'http://localhost:5259/api';

  constructor(private http: HttpClient) {}

  getCityDashboard(city: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/dashboard/city/${city}`);
  }

  updateBedCount(data: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/bed/update`, data);
  }
}
