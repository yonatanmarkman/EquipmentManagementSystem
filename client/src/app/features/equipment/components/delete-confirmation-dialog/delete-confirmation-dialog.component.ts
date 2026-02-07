import { Component, inject } from '@angular/core';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-delete-confirmation-dialog',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule, MatIconModule],
  template: `
    <h2 mat-dialog-title>
      <mat-icon color="warn">warning</mat-icon>
      Confirm Deletion
    </h2>
    <mat-dialog-content>
      <p>Are you sure you want to delete this equipment item?</p>
      <p class="warning-text">This action cannot be undone.</p>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Cancel</button>
      <button mat-raised-button color="warn" [mat-dialog-close]="true">
        <mat-icon>delete</mat-icon>
        Delete
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    h2 {
      display: flex;
      align-items: center;
      gap: 12px;
    }

    mat-icon {
      font-size: 32px;
      width: 32px;
      height: 32px;
    }

    .warning-text {
      color: #f44336;
      font-weight: 500;
      margin-top: 8px;
    }

    mat-dialog-actions {
      padding: 16px 24px;
    }
  `]
})
export class DeleteConfirmationDialogComponent {
  dialogRef = inject(MatDialogRef<DeleteConfirmationDialogComponent>);
}