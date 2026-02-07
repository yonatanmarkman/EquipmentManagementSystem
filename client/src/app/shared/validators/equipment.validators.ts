import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export class EquipmentValidators {
  // Serial number format: Must contain letters and numbers, 3-100 characters
  static serialNumberFormat(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) {
        return null; // Don't validate empty values, use Validators.required for that
      }

      const serialNumber = control.value as string;
      const hasLetters = /[a-zA-Z]/.test(serialNumber);
      const hasNumbers = /[0-9]/.test(serialNumber);
      const validLength = serialNumber.length >= 3 && serialNumber.length <= 100;

      if (!hasLetters || !hasNumbers || !validLength) {
        return { invalidSerialNumber: true };
      }

      return null;
    };
  }

  // Purchase date must be today or in the past
  static purchaseDateNotFuture(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) {
        return null;
      }

      const selectedDate = new Date(control.value);
      const today = new Date();
      today.setHours(0, 0, 0, 0);
      selectedDate.setHours(0, 0, 0, 0);

      if (selectedDate > today) {
        return { futureDate: true };
      }

      return null;
    };
  }
}