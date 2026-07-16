import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { VisitorService } from '../../services/visitor.service';
import { VisitorCheckInDto } from '../../models/visitor';

@Component({
  selector: 'app-check-in',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './check-in.html',
  styleUrl: './check-in.css',
})
export class CheckIn {
  private readonly fb = inject(FormBuilder);
  private readonly visitorService = inject(VisitorService);
  private readonly router = inject(Router);

  // Signals for handling form state
  isSubmitting = signal<boolean>(false);
  errorMessage = signal<string | null>(null);

  // Form Group configuration
  // The phone pattern regex matches: optional +91, optional space/dash, optional 91, and a 10-digit number starting with 6-9.
  checkInForm: FormGroup = this.fb.group({
    fullName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(150)]],
    phoneNumber: ['', [
      Validators.required,
      Validators.pattern(/^(\+91[\-\s]?)?[0]?(91)?[6-9]\d{9}$/)
    ]],
    company: ['', [Validators.maxLength(100)]],
    hostName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
    purpose: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(200)]],
    badgeNumber: ['', [Validators.maxLength(20)]]
  });

  // Helper getters to simplify template error checks
  get f() {
    return this.checkInForm.controls;
  }

  onSubmit(): void {
    if (this.checkInForm.invalid) {
      // Mark all controls as touched to display validation styling/errors
      this.checkInForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    const checkInPayload: VisitorCheckInDto = this.checkInForm.value;

    this.visitorService.checkIn(checkInPayload).subscribe({
      next: (createdVisitor) => {
        this.isSubmitting.set(false);
        // Successful check-in! Redirect to dashboard to show active guest
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        console.error('Error checking in visitor:', err);
        this.isSubmitting.set(false);
        
        // Handle server-side validation messages if any
        if (err.status === 400 && err.error?.errors) {
          const validationErrors = err.error.errors;
          const firstErrorKey = Object.keys(validationErrors)[0];
          const firstErrorMsg = validationErrors[firstErrorKey][0];
          this.errorMessage.set(`Validation error: ${firstErrorMsg}`);
        } else {
          this.errorMessage.set('Check-in failed. Please verify the backend is running and try again.');
        }
      }
    });
  }

  resetForm(): void {
    this.checkInForm.reset();
    this.errorMessage.set(null);
  }
}
