import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { VisitorService } from '../../services/visitor.service';
import { Visitor } from '../../models/visitor';

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard implements OnInit {
  private readonly visitorService = inject(VisitorService);

  // Signal holding the list of active visitors in the building
  activeVisitors = signal<Visitor[]>([]);
  
  // Signals for loading and error state
  isLoading = signal<boolean>(true);
  errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.loadActiveVisitors();
  }

  loadActiveVisitors(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.visitorService.getActiveVisitors().subscribe({
      next: (data) => {
        this.activeVisitors.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Error fetching active visitors:', err);
        this.errorMessage.set('Failed to load active visitors. Please verify backend is running.');
        this.isLoading.set(false);
      }
    });
  }

  checkOutVisitor(id: number): void {
    this.visitorService.checkOut(id).subscribe({
      next: (updatedVisitor) => {
        // Update local signal state by filtering out the checked-out visitor
        this.activeVisitors.update(visitors => 
          visitors.filter(v => v.id !== id)
        );
      },
      error: (err) => {
        console.error('Error checking out visitor:', err);
        alert('Check-out failed. Please try again.');
      }
    });
  }
}
