import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

// Angular Material imports
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatChipsModule } from '@angular/material/chips';

import { EquipmentStateService } from '../../../../services/equipment-state.service';
import { EquipmentStatus } from '../../../../types';

@Component({
  selector: 'app-equipment-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatProgressSpinnerModule,
    MatPaginatorModule,
    MatChipsModule
  ],
  templateUrl: './equipment-list.component.html',
  styleUrls: ['./equipment-list.component.css']
})
export class EquipmentListComponent implements OnInit {
  equipmentState = inject(EquipmentStateService);

  // Expose for template
  displayedColumns: string[] = ['equipmentName', 'serialNumber', 'categoryName', 'locationName', 'status', 'actions'];
  equipmentStatuses = Object.values(EquipmentStatus);

  ngOnInit() {
    this.equipmentState.loadEquipment();
    this.equipmentState.loadCategories();
    this.equipmentState.loadLocations();
  }

  onCategoryChange(categoryId: number | null) {
    this.equipmentState.setCategory(categoryId);
  }

  onStatusChange(status: EquipmentStatus | null) {
    this.equipmentState.setStatus(status);
  }

  onSearchNameChange(name: string) {
    this.equipmentState.setSearchName(name);
  }

  onSearchDateChange(date: Date | null) {
    this.equipmentState.setSearchDate(date);
  }

  clearFilters() {
    this.equipmentState.clearFilters();
  }

  onPageChange(event: any) {
    this.equipmentState.goToPage(event.pageIndex + 1);
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
}