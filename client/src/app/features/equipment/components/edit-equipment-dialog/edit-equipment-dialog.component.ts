import { Component, OnInit, inject, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';

// Angular Material imports
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { Equipment, UpdateEquipment, EquipmentStatus, Category, Location } from '../../../../types';
import { EquipmentValidators } from '../../../../shared/validators/equipment.validators';

@Component({
  selector: 'app-edit-equipment-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './edit-equipment-dialog.component.html',
  styleUrls: ['./edit-equipment-dialog.component.css']
})
export class EditEquipmentDialogComponent implements OnInit {
  private fb = inject(FormBuilder);
  dialogRef = inject(MatDialogRef<EditEquipmentDialogComponent>);

  equipmentForm!: FormGroup;
  equipmentStatuses = Object.values(EquipmentStatus);

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { 
      equipment: Equipment, 
      categories: Category[], 
      locations: Location[] 
    }
  ) {}

  ngOnInit() {
    console.log('Equipment data:', this.data.equipment);
    console.log('Equipment status:', this.data.equipment.status);
    console.log('Equipment statuses enum:', this.equipmentStatuses);
    this.initializeForm();
  }

  initializeForm() {
    const equipment = this.data.equipment;
    
    // Ensure status matches enum value
    const statusValue = equipment.status as EquipmentStatus;

    this.equipmentForm = this.fb.group({
      equipmentName: [
        equipment.equipmentName,
        [
          Validators.required,
          Validators.minLength(3),
          Validators.maxLength(200)
        ]
      ],
      serialNumber: [
        equipment.serialNumber,
        [
          Validators.required,
          Validators.minLength(3),
          Validators.maxLength(100),
          EquipmentValidators.serialNumberFormat()
        ]
      ],
      categoryId: [equipment.categoryId, [Validators.required]],
      locationId: [equipment.locationId, [Validators.required]],
      purchaseDate: [
        new Date(equipment.purchaseDate),
        [
          Validators.required,
          EquipmentValidators.purchaseDateNotFuture()
        ]
      ],
      status: [equipment.status, [Validators.required]]
    });

    // Debug: check if status is set correctly
    console.log('Form status value:', this.equipmentForm.get('status')?.value);
  }

  onSubmit() {
    console.log('Form valid:', this.equipmentForm.valid);
    console.log('Form value:', this.equipmentForm.value);
    console.log('Form errors:', this.equipmentForm.errors);

    if (this.equipmentForm.invalid) {
      this.equipmentForm.markAllAsTouched();
      console.log('Form is invalid, not closing dialog');
      return;
    }

    const formValue = this.equipmentForm.value;
    const updateDto: UpdateEquipment = {
      equipmentName: formValue.equipmentName,
      serialNumber: formValue.serialNumber,
      categoryId: formValue.categoryId,
      locationId: formValue.locationId,
      purchaseDate: formValue.purchaseDate,
      status: formValue.status
    };

    this.dialogRef.close(updateDto);
  }

  onCancel() {
    this.dialogRef.close();
  }

  getErrorMessage(fieldName: string): string {
    const control = this.equipmentForm.get(fieldName);
    
    if (!control || !control.errors || !control.touched) {
      return '';
    }

    if (control.hasError('required')) {
      return `${this.getFieldLabel(fieldName)} is required`;
    }

    if (control.hasError('minlength')) {
      const minLength = control.errors['minlength'].requiredLength;
      return `${this.getFieldLabel(fieldName)} must be at least ${minLength} characters`;
    }

    if (control.hasError('maxlength')) {
      const maxLength = control.errors['maxlength'].requiredLength;
      return `${this.getFieldLabel(fieldName)} must not exceed ${maxLength} characters`;
    }

    if (control.hasError('invalidSerialNumber')) {
      return 'Serial number must contain both letters and numbers';
    }

    if (control.hasError('futureDate')) {
      return 'Purchase date cannot be in the future';
    }

    return 'Invalid value';
  }

  private getFieldLabel(fieldName: string): string {
    const labels: { [key: string]: string } = {
      equipmentName: 'Equipment name',
      serialNumber: 'Serial number',
      categoryId: 'Category',
      locationId: 'Location',
      purchaseDate: 'Purchase date',
      status: 'Status'
    };
    return labels[fieldName] || fieldName;
  }

  hasError(fieldName: string): boolean {
    const control = this.equipmentForm.get(fieldName);
    return !!(control && control.invalid && control.touched);
  }
}