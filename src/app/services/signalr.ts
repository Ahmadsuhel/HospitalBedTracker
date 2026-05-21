import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';  // ← yeh add karo
import { BehaviorSubject } from 'rxjs';


@Injectable({
  providedIn: 'root',
})
export class SignalrService {
  private hubConnection!: signalR.HubConnection;
  
  bedUpdated$ = new BehaviorSubject<any>(null);
  criticalAlert$ = new BehaviorSubject<any>(null);

  startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5259/hubs/beds')
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start()
      .then(() => console.log('SignalR Connected!'))
      .catch(err => console.error('SignalR Error:', err));

    this.hubConnection.on('BedCountUpdated', (data) => {
      this.bedUpdated$.next(data);
    });

    this.hubConnection.on('CriticalAlert', (alert) => {
      this.criticalAlert$.next(alert);
    });
  }

  joinCityGroup(city: string) {
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.invoke('JoinCityGroup', city);
    }
  }
}