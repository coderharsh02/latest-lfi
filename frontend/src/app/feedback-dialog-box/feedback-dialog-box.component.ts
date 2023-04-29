import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

export interface FeedbackData {
  question: string;
  answer: string;
}

@Component({
  selector: 'app-feedback-dialog-box',
  templateUrl: './feedback-dialog-box.component.html',
  styleUrls: ['./feedback-dialog-box.component.css'],
})
export class FeedbackDialogBoxComponent {
  constructor(
    public dialogRef: MatDialogRef<FeedbackDialogBoxComponent>,
    @Inject(MAT_DIALOG_DATA) public data: FeedbackData
  ) {}

  onNoClick(): void {
    this.dialogRef.close();
  }
}
