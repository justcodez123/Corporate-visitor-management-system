import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Visitor, VisitorCheckInDto } from '../models/visitor';

@Injectable({
  providedIn: 'root',
})
export class VisitorService {
  // Use the new inject() function instead of constructor injection.
  // This is the recommended pattern in modern Angular (v16+).
  private readonly http = inject(HttpClient);
  
  // URL pointing to our ASP.NET Core Web API backend
  private readonly apiUrl = 'http://localhost:5147/api/visitors';

  /**
   * Get all visitors, optionally filtering by name.
   */
  getVisitors(search?: string): Observable<Visitor[]> {
    const url = search ? `${this.apiUrl}?search=${encodeURIComponent(search)}` : this.apiUrl;
    return this.http.get<Visitor[]>(url);
  }

  /**
   * Get all visitors currently in the building (no checkout time).
   */
  getActiveVisitors(): Observable<Visitor[]> {
    return this.http.get<Visitor[]>(`${this.apiUrl}/active`);
  }

  /**
   * Check in a new visitor.
   */
  checkIn(visitor: VisitorCheckInDto): Observable<Visitor> {
    return this.http.post<Visitor>(this.apiUrl, visitor);
  }

  /**
   * Check out a visitor by setting their checkout timestamp.
   */
  checkOut(id: number): Observable<Visitor> {
    return this.http.put<Visitor>(`${this.apiUrl}/${id}/checkout`, {});
  }

  /**
   * Delete a visitor record permanently.
   */
  deleteVisitor(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
