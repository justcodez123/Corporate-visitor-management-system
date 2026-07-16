import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./pages/dashboard/dashboard').then(m => m.Dashboard)
  },
  {
    path: 'check-in',
    loadComponent: () => import('./pages/check-in/check-in').then(m => m.CheckIn)
  },
  {
    path: 'search',
    loadComponent: () => import('./pages/search/search').then(m => m.Search)
  },
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];
