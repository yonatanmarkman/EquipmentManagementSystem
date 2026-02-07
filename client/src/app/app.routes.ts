import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'equipment',
    pathMatch: 'full'
  },
  {
    path: 'equipment',
    loadComponent: () => import('./features/equipment/pages/equipment-list/equipment-list.component').then(m => m.EquipmentListComponent)
  },
  {
    path: 'equipment/new',
    loadComponent: () => import('./features/equipment/pages/add-equipment/add-equipment.component').then(m => m.AddEquipmentComponent)
  },
  {
    path: 'equipment/:id',
    loadComponent: () => import('./features/equipment/pages/equipment-detail/equipment-detail.component').then(m => m.EquipmentDetailComponent)
  },
  {
    path: '**',
    redirectTo: 'equipment'
  }
];