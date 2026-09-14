import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', loadComponent: () => import('./pages/home/home').then((m) => m.Home) },
  { path: 'rsvp/:token', loadComponent: () => import('./pages/rsvp/rsvp').then((m) => m.Rsvp) },
  { path: 'admin', loadComponent: () => import('./pages/admin/admin').then((m) => m.Admin) },
  { path: '**', redirectTo: '' },
];
