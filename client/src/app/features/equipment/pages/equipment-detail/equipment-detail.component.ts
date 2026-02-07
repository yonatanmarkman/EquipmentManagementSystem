import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';

// Angular Material imports
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { ApiService } from '../../../../services/api.service';
import { EquipmentStateService } from '../../../../services/equipment-state.service';
import { Equipment, EquipmentStatus } from '../../../../types';
import { EditEquipmentDialogComponent } from '../../components/edit-equipment-dialog/edit-equipment-dialog.component';
import { DeleteConfirmationDialogComponent } from '../../components/delete-confirmation-dialog/delete-confirmation-dialog.component';

@Component({
  selector: 'app-equipment-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatChipsModule,
    MatDialogModule,
    MatSnackBarModule
  ],
  templateUrl: './equipment-detail.component.html',
  styleUrls: ['./equipment-detail.component.css']
})
export class EquipmentDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private apiService = inject(ApiService);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);
  equipmentState = inject(EquipmentStateService);

  equipment = signal<Equipment | null>(null);
  loading = signal<boolean>(false);
  error = signal<string | null>(null);

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.loadEquipment(id);
      this.equipmentState.loadCategories();
      this.equipmentState.loadLocations();
    }
  }

  loadEquipment(id: number) {
    this.loading.set(true);
    this.error.set(null);

    this.apiService.getEquipmentById(id).subscribe({
      next: (equipment) => {
        this.equipment.set(equipment);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading equipment:', err);
        this.error.set('Failed to load equipment details');
        this.loading.set(false);
      }
    });
  }

  openEditDialog() {
    const dialogRef = this.dialog.open(EditEquipmentDialogComponent, {
      width: '600px',
      data: {
        equipment: this.equipment(),
        categories: this.equipmentState.categories(),
        locations: this.equipmentState.locations()
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.updateEquipment(result);
      }
    });
  }

  updateEquipment(updateDto: any) {
    const id = this.equipment()?.id;
    if (!id) return;

    this.apiService.updateEquipment(id, updateDto).subscribe({
      next: (updatedEquipment) => {
        this.equipment.set(updatedEquipment);
        this.snackBar.open('Equipment updated successfully!', 'Close', {
          duration: 3000,
          panelClass: ['success-snackbar']
        });
      },
      error: (error) => {
        console.error('Error updating equipment:', error);
        const errorMessage = error.error?.message || 'Failed to update equipment';
        this.snackBar.open(errorMessage, 'Close', {
          duration: 5000,
          panelClass: ['error-snackbar']
        });
      }
    });
  }

  openDeleteDialog() {
    const dialogRef = this.dialog.open(DeleteConfirmationDialogComponent, {
      width: '400px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.deleteEquipment();
      }
    });
  }

  deleteEquipment() {
    const id = this.equipment()?.id;
    if (!id) return;

    this.equipmentState.deleteEquipment(id).subscribe({
      next: () => {
        this.snackBar.open('Equipment deleted successfully!', 'Close', {
          duration: 3000,
          panelClass: ['success-snackbar']
        });
        this.router.navigate(['/equipment']);
      },
      error: (error) => {
        console.error('Error deleting equipment:', error);
        const errorMessage = error.error?.message || 'Failed to delete equipment';
        this.snackBar.open(errorMessage, 'Close', {
          duration: 5000,
          panelClass: ['error-snackbar']
        });
      }
    });
  }

  getStatusColor(status: EquipmentStatus): string {
    switch (status) {
      case EquipmentStatus.Active:
        return 'primary';
      case EquipmentStatus.InMaintenance:
        return 'accent';
      case EquipmentStatus.OutOfService:
        return 'warn';
      case EquipmentStatus.Retired:
        return '';
      default:
        return '';
    }
  }

  goBack() {
    this.router.navigate(['/equipment']);
  }
}