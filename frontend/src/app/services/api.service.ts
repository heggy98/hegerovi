import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Guest } from '../models/guest.model';
import { InvitationOverview, RsvpInfo, RsvpRequest } from '../models/invitation.model';
import { AdminAuthService } from './admin-auth.service';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly adminAuth = inject(AdminAuthService);
  private readonly baseUrl = environment.apiBaseUrl;

  private adminHeaders(): HttpHeaders {
    return new HttpHeaders({ 'x-admin-key': this.adminAuth.apiKey ?? '' });
  }

  getRsvp(token: string): Observable<RsvpInfo> {
    return this.http.get<RsvpInfo>(`${this.baseUrl}/rsvp/${token}`);
  }

  submitRsvp(token: string, request: RsvpRequest): Observable<{ status: string }> {
    return this.http.post<{ status: string }>(`${this.baseUrl}/rsvp/${token}`, request);
  }

  getGuests(): Observable<Guest[]> {
    return this.http.get<Guest[]>(`${this.baseUrl}/guests`, { headers: this.adminHeaders() });
  }

  createGuest(guest: Partial<Guest>): Observable<{ guest: Guest }> {
    return this.http.post<{ guest: Guest }>(`${this.baseUrl}/guests`, guest, { headers: this.adminHeaders() });
  }

  updateGuest(id: string, guest: Partial<Guest>): Observable<Guest> {
    return this.http.put<Guest>(`${this.baseUrl}/guests/${id}`, guest, { headers: this.adminHeaders() });
  }

  deleteGuest(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/guests/${id}`, { headers: this.adminHeaders() });
  }

  getInvitations(): Observable<InvitationOverview[]> {
    return this.http.get<InvitationOverview[]>(`${this.baseUrl}/invitations`, { headers: this.adminHeaders() });
  }
}
