import { Component, OnInit, OnDestroy, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { Subscription, debounceTime, distinctUntilChanged, switchMap, tap } from 'rxjs';
import { VisitorService } from '../../services/visitor.service';
import { Visitor } from '../../models/visitor';

@Component({
  selector: 'app-search',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './search.html',
  styleUrl: './search.css',
})
export class Search implements OnInit, OnDestroy {
  private readonly visitorService = inject(VisitorService);

  // Search input control
  searchControl = new FormControl('');
  
  // Results signal
  searchResults = signal<Visitor[]>([]);
  
  // Loading and helper state signals
  isLoading = signal<boolean>(false);
  hasSearched = signal<boolean>(false);
  errorMessage = signal<string | null>(null);

  private searchSubscription?: Subscription;

  ngOnInit(): void {
    // Load initial list (all visitors)
    this.performSearch('');

    // Set up debounced search stream
    // This is a crucial RxJS concept to prevent API spamming:
    // - debounceTime(300): wait for 300ms of no typing before emitting.
    // - distinctUntilChanged(): only search if the query actually changed.
    // - switchMap(): cancel any pending API call if a new one is made.
    this.searchSubscription = this.searchControl.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      tap(() => this.isLoading.set(true)),
      switchMap(query => this.visitorService.getVisitors(query ?? ''))
    ).subscribe({
      next: (data) => {
        this.searchResults.set(data);
        this.isLoading.set(false);
        this.hasSearched.set(true);
      },
      error: (err) => {
        console.error('Search query stream error:', err);
        this.errorMessage.set('Failed to search logs. Please try again.');
        this.isLoading.set(false);
      }
    });
  }

  ngOnDestroy(): void {
    // Always clean up subscriptions to avoid memory leaks!
    this.searchSubscription?.unsubscribe();
  }

  performSearch(query: string): void {
    this.isLoading.set(true);
    this.visitorService.getVisitors(query).subscribe({
      next: (data) => {
        this.searchResults.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Initial load error:', err);
        this.isLoading.set(false);
      }
    });
  }

  checkOutFromSearch(id: number): void {
    this.visitorService.checkOut(id).subscribe({
      next: (updatedVisitor) => {
        // Update local search results signal to reflect the checkout
        this.searchResults.update(visitors => 
          visitors.map(v => v.id === id ? updatedVisitor : v)
        );
      },
      error: (err) => {
        console.error('Checkout error:', err);
        alert('Check-out failed. Please try again.');
      }
    });
  }

  deleteRecord(id: number): void {
    if (!confirm('Are you sure you want to permanently delete this visitor log?')) {
      return;
    }

    this.visitorService.deleteVisitor(id).subscribe({
      next: () => {
        // Remove the deleted visitor from local results
        this.searchResults.update(visitors => 
          visitors.filter(v => v.id !== id)
        );
      },
      error: (err) => {
        console.error('Delete error:', err);
        alert('Failed to delete record.');
      }
    });
  }

  /**
   * Helper to format check-in/check-out durations
   */
  getDuration(checkIn: string, checkOut?: string | null): string {
    if (!checkOut) return 'Active';
    
    const start = new Date(checkIn);
    const end = new Date(checkOut);
    const diffMs = end.getTime() - start.getTime();
    
    const diffMins = Math.floor(diffMs / 1000 / 60);
    if (diffMins < 1) return '< 1 min';
    if (diffMins < 60) return `${diffMins} min`;
    
    const hours = Math.floor(diffMins / 60);
    const mins = diffMins % 60;
    return `${hours}h ${mins}m`;
  }
}
