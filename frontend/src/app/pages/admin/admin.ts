import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AdminAuthService } from '../../services/admin-auth.service';
import { ApiService } from '../../services/api.service';
import { Guest } from '../../models/guest.model';
import { InvitationOverview } from '../../models/invitation.model';

type Tab = 'guests' | 'rsvp';

@Component({
  selector: 'app-admin',
  imports: [FormsModule],
  templateUrl: './admin.html',
  styleUrl: './admin.scss',
})
export class Admin {
  private readonly api = inject(ApiService);
  readonly auth = inject(AdminAuthService);

  readonly tab = signal<Tab>('guests');
  readonly guests = signal<Guest[]>([]);
  readonly invitations = signal<InvitationOverview[]>([]);
  readonly loginError = signal<string | null>(null);
  readonly loading = signal(false);

  apiKeyInput = '';
  newGuestName = '';
  newGuestGroup = '';
  newGuestPlusOne = false;

  login(): void {
    this.loginError.set(null);
    this.auth.login(this.apiKeyInput.trim());
    this.api.getGuests().subscribe({
      next: (guests) => {
        this.guests.set(guests);
        this.refreshInvitations();
      },
      error: () => {
        this.auth.logout();
        this.loginError.set('Neplatný přístupový klíč.');
      },
    });
  }

  logout(): void {
    this.auth.logout();
    this.guests.set([]);
    this.invitations.set([]);
  }

  selectTab(tab: Tab): void {
    this.tab.set(tab);
    if (tab === 'rsvp') {
      this.refreshInvitations();
    }
  }

  private refreshGuests(): void {
    this.api.getGuests().subscribe((guests) => this.guests.set(guests));
  }

  private refreshInvitations(): void {
    this.api.getInvitations().subscribe((invitations) => this.invitations.set(invitations));
  }

  addGuest(): void {
    if (!this.newGuestName.trim()) {
      return;
    }

    this.api
      .createGuest({
        name: this.newGuestName.trim(),
        group: this.newGuestGroup.trim(),
        plusOneAllowed: this.newGuestPlusOne,
      })
      .subscribe(() => {
        this.newGuestName = '';
        this.newGuestGroup = '';
        this.newGuestPlusOne = false;
        this.refreshGuests();
      });
  }

  deleteGuest(id: string): void {
    this.api.deleteGuest(id).subscribe(() => this.refreshGuests());
  }

  onCsvSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) {
      return;
    }

    this.loading.set(true);
    file.text().then((text) => {
      const rows = text
        .split('\n')
        .map((row) => row.trim())
        .filter((row) => row.length > 0);

      let pending = rows.length;
      if (pending === 0) {
        this.loading.set(false);
        return;
      }

      for (const row of rows) {
        const [name, group, plusOne] = row.split(',').map((cell) => cell.trim());
        this.api
          .createGuest({
            name,
            group: group ?? '',
            plusOneAllowed: (plusOne ?? '').toLowerCase() === 'ano',
          })
          .subscribe({
            next: () => {
              pending -= 1;
              if (pending === 0) {
                this.loading.set(false);
                this.refreshGuests();
              }
            },
            error: () => {
              pending -= 1;
              if (pending === 0) {
                this.loading.set(false);
                this.refreshGuests();
              }
            },
          });
      }
    });

    input.value = '';
  }

  exportCsv(): void {
    const header = 'Jméno,Stav,Počet osob,Menu,Alergie,Token\n';
    const rows = this.invitations()
      .map((inv) =>
        [inv.guestName ?? '', inv.status, inv.peopleCount, inv.menuPreference ?? '', inv.allergies ?? '', inv.token].join(','),
      )
      .join('\n');

    const blob = new Blob([header + rows], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = 'rsvp-export.csv';
    link.click();
    URL.revokeObjectURL(url);
  }
}
