import { Injectable, signal } from '@angular/core';

const STORAGE_KEY = 'hegerovi-admin-key';

@Injectable({ providedIn: 'root' })
export class AdminAuthService {
  private readonly apiKeySignal = signal<string | null>(sessionStorage.getItem(STORAGE_KEY));

  readonly isLoggedIn = signal(this.apiKeySignal() !== null);

  get apiKey(): string | null {
    return this.apiKeySignal();
  }

  login(apiKey: string): void {
    sessionStorage.setItem(STORAGE_KEY, apiKey);
    this.apiKeySignal.set(apiKey);
    this.isLoggedIn.set(true);
  }

  logout(): void {
    sessionStorage.removeItem(STORAGE_KEY);
    this.apiKeySignal.set(null);
    this.isLoggedIn.set(false);
  }
}
