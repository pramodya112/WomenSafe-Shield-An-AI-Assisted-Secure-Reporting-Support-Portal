import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { trigger, transition, style, animate } from '@angular/animations';

interface TrackedReport {
  id: number;
  title: string;
  incidentType: string;
  status: any;           // may be number OR string depending on API
  statusDisplay: string; // always a string e.g. "Pending"
  createdAt: string;
  updatedAt?: string;
  adminNotes?: string;
}

@Component({
  selector: 'app-track-report',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './track-report.component.html',
  styleUrls: ['./track-report.component.css'],
  animations: [
    trigger('fadeIn', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(16px)' }),
        animate('350ms ease-out', style({ opacity: 1, transform: 'translateY(0)' }))
      ])
    ])
  ]
})
export class TrackReportComponent {

  trackingCode: string = '';
  isLoading: boolean = false;
  searchError: string = '';
  report: TrackedReport | null = null;
  notFound: boolean = false;
  lastSearchedCode: string = '';

  private apiBase = 'http://localhost:5032/api';

  constructor(private http: HttpClient) { }

  onCodeChange(): void {
    let val = this.trackingCode.toUpperCase().replace(/[^A-Z0-9-]/g, '');
    this.trackingCode = val;
    this.searchError = '';
    if (this.report || this.notFound) {
      this.report = null;
      this.notFound = false;
    }
  }

  trackReport(): void {
    const code = this.trackingCode.trim().toUpperCase();
    if (!code) { this.searchError = 'Please enter a tracking code.'; return; }
    if (code.length < 5) { this.searchError = 'Tracking code is too short.'; return; }

    this.isLoading = true;
    this.report = null;
    this.notFound = false;
    this.searchError = '';
    this.lastSearchedCode = code;

    this.http.get<TrackedReport>(`${this.apiBase}/reports/track/${code}`)
      .subscribe({
        next: (data) => {
          // Normalize: if statusDisplay missing, convert numeric status to string
          if (!data.statusDisplay) {
            const statusMap: Record<number, string> = {
              0: 'Pending',
              1: 'UnderReview',
              2: 'AssignmentToGo',
              3: 'Closed'
            };
            data.statusDisplay = typeof data.status === 'number'
              ? (statusMap[data.status] ?? 'Pending')
              : String(data.status);
          }
          this.report = data;
          this.isLoading = false;
          setTimeout(() => {
            document.querySelector('.tr-result-card')?.scrollIntoView({ behavior: 'smooth', block: 'start' });
          }, 100);
        },
        error: (err) => {
          this.isLoading = false;
          if (err.status === 404) this.notFound = true;
          else this.searchError = 'Something went wrong. Please try again later.';
        }
      });
  }

  clearSearch(): void {
    this.trackingCode = '';
    this.report = null;
    this.notFound = false;
    this.searchError = '';
    this.lastSearchedCode = '';
  }

  // Use statusDisplay (always a string) for all comparisons
  getStatusClass(report: TrackedReport): string {
    const s = report.statusDisplay;
    const map: Record<string, string> = {
      'Pending': 'status-pending',
      'UnderReview': 'status-review',
      'AssignmentToGo': 'status-action',
      'Closed': 'status-closed'
    };
    return map[s] ?? 'status-pending';
  }

  getStatusIcon(report: TrackedReport): string {
    const s = report.statusDisplay;
    const map: Record<string, string> = {
      'Pending': 'bi-hourglass-split',
      'UnderReview': 'bi-eye-fill',
      'AssignmentToGo': 'bi-lightning-fill',
      'Closed': 'bi-check-circle-fill'
    };
    return map[s] ?? 'bi-hourglass-split';
  }

  getStatusDisplay(report: TrackedReport): string {
    const s = report.statusDisplay;
    const map: Record<string, string> = {
      'Pending': 'Pending Review',
      'UnderReview': 'Under Review',
      'AssignmentToGo': 'In Action',
      'Closed': 'Resolved / Closed'
    };
    return map[s] ?? s;
  }

  isStepDone(step: string, report: TrackedReport): boolean {
    const order = ['Pending', 'UnderReview', 'AssignmentToGo', 'Closed'];
    return order.indexOf(report.statusDisplay) >= order.indexOf(step);
  }

  isCurrentStep(step: string, report: TrackedReport): boolean {
    return report.statusDisplay === step;
  }
}
