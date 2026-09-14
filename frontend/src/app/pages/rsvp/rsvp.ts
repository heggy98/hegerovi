import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { RsvpInfo } from '../../models/invitation.model';

@Component({
  selector: 'app-rsvp',
  imports: [FormsModule],
  templateUrl: './rsvp.html',
  styleUrl: './rsvp.scss',
})
export class Rsvp {
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(ApiService);

  private readonly token = this.route.snapshot.paramMap.get('token') ?? '';

  readonly loading = signal(true);
  readonly notFound = signal(false);
  readonly submitted = signal(false);
  readonly error = signal<string | null>(null);
  readonly info = signal<RsvpInfo | null>(null);

  attending = true;
  peopleCount = 1;
  menuPreference = '';
  allergies = '';

  constructor() {
    this.loadInvitation();
  }

  private loadInvitation(): void {
    this.api.getRsvp(this.token).subscribe({
      next: (info) => {
        this.info.set(info);
        this.peopleCount = info.peopleCount || 1;
        this.menuPreference = info.menuPreference ?? '';
        this.allergies = info.allergies ?? '';
        this.attending = info.status !== 'Declined';
        this.submitted.set(info.status !== 'Pending');
        this.loading.set(false);
      },
      error: () => {
        this.notFound.set(true);
        this.loading.set(false);
      },
    });
  }

  submit(): void {
    this.error.set(null);
    this.api
      .submitRsvp(this.token, {
        attending: this.attending,
        peopleCount: this.attending ? this.peopleCount : 0,
        menuPreference: this.attending ? this.menuPreference : undefined,
        allergies: this.attending ? this.allergies : undefined,
      })
      .subscribe({
        next: () => this.submitted.set(true),
        error: () => this.error.set('Odeslání se nepovedlo, zkuste to prosím znovu.'),
      });
  }

  editAgain(): void {
    this.submitted.set(false);
  }
}
