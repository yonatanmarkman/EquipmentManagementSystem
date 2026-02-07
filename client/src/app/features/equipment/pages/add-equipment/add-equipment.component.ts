import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';

// Angular Material imports
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { ApiService } from '../../../../services/api.service';
import { EquipmentStateService } from '../../../../services/equipment-state.service';
import { CreateEquipment, EquipmentStatus } from '../../../../types';
import { EquipmentValidators } from '../../../../shared/validators/equipment.validators';

@Component({
  selector: 'app-add-equipment',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatSnackBarModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './add-equipment.component.html',
  styleUrls: ['./add-equipment.component.css']
})
export class AddEquipmentComponent implements OnInit {
  private fb = inject(FormBuilder);
  private apiService = inject(ApiService);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);
  equipmentState = inject(EquipmentStateService);

  equipmentForm!: FormGroup;
  isSubmitting = false;
  equipmentStatuses = Object.values(EquipmentStatus);

  ngOnInit() {
    this.equipmentState.loadCategories();
    this.equipmentState.loadLocations();
    this.initializeForm();
  }

  initializeForm() {
    this.equipmentForm = this.fb.group({
      equipmentName: [
        '',
        [
          Validators.required,
          Validators.minLength(3),
          Validators.maxLength(200)
        ]
      ],
      serialNumber: [
        '',
        [
          Validators.required,
          Validators.minLength(3),
          Validators.maxLength(100),
          EquipmentValidators.serialNumberFormat()
        ]
      ],
      categoryId: [null, [Validators.required]],
      locationId: [null, [Validators.required]],
      purchaseDate: [
        null,
        [
          Validators.required,
          EquipmentValidators.purchaseDateNotFuture()
        ]
      ],
      status: [EquipmentStatus.Active, [Validators.required]]
    });
  }

  onSubmit() {
    console.log('=== FORM DEBUG ===');
    console.log('Form valid:', this.equipmentForm.valid);
    console.log('Form value:', this.equipmentForm.value);
    console.log('Form errors:', this.equipmentForm.errors);
    
    // Check each field
    Object.keys(this.equipmentForm.controls).forEach(key => {
      const control = this.equipmentForm.get(key);
      if (control && control.invalid) {
        console.log(`Field '${key}' is INVALID:`, control.errors);
      }
    });

    if (this.equipmentForm.invalid) {
      this.equipmentForm.markAllAsTouched();
      this.snackBar.open('Please fix the form errors', 'Close', {
        duration: 3000,
        panelClass: ['error-snackbar']
      });
      return;
    }

    this.isSubmitting = true;

    const formValue = this.equipmentForm.value;
    const createDto: CreateEquipment = {
      equipmentName: formValue.equipmentName,
      serialNumber: formValue.serialNumber,
      categoryId: formValue.categoryId,
      locationId: formValue.locationId,
      purchaseDate: formValue.purchaseDate.toISOString(),
      status: formValue.status
    };

    // Print the payload to the console log
    console.log('Sending to API:', JSON.stringify(createDto, null, 2));

    this.apiService.createEquipment(createDto).subscribe({
      next: () => {
        this.snackBar.open('Equipment created successfully!', 'Close', {
          duration: 3000,
          panelClass: ['success-snackbar']
        });
        this.router.navigate(['/equipment']);
      },
      error: (error) => {
        this.isSubmitting = false; 
        console.error('Error creating equipment:', error);
        console.error('Error details:', error.error);
        const errorMessage = error.error?.message || 'Failed to create equipment';
        this.snackBar.open(errorMessage, 'Close', {
          duration: 5000,
          panelClass: ['error-snackbar']
        });
      },
      complete: () => {
        this.isSubmitting = false;
      }
    });
  }

  onCancel() {
    this.router.navigate(['/equipment']);
  }

  // Helper methods for error messages
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