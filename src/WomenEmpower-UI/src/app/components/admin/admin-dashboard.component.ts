import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';

interface Report {
  id: string;
  anonymousTrackingCode: string;
  title: string;
  description?: string;
  incidentType: string;
  location?: string;
  status: string;
  severity?: string;
  createdAt: string;
  updatedAt?: string;
  adminNotes?: string;
  contactInfo?: string;
  victimName?: string;
  victimAge?: number;
  reporterType?: string;
  relationship?: string;
  incidentDate?: string;
  incidentTime?: string;
  additionalNotes?: string;
  evidenceCount?: number;
  // UI state
  _draftNotes?: string;
}

interface Stats {
  total: number;
  pending: number;
  underReview: number;
  inAction: number;
  closed: number;
}

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css'],
})
export class AdminDashboardComponent implements OnInit {

  private apiBase = 'http://localhost:5032/api';

  reports: Report[] = [];
  filteredReports: Report[] = [];
  isLoading = false;
  loadError = '';
  today = new Date();

  // Sidebar
  sidebarCollapsed = false;

  // Filters
  searchQuery = '';
  statusFilter = '';
  sortField = 'createdAt';
  sortAsc = false;

  // Expand
  expandedId: string | null = null;

  // Stats
  stats: Stats = { total: 0, pending: 0, underReview: 0, inAction: 0, closed: 0 };

  // Status options for buttons
  statusOptions = [
    { value: 'Pending', label: 'Pending', icon: 'bi-hourglass-split' },
    { value: 'UnderReview', label: 'Under Review', icon: 'bi-eye-fill' },
    { value: 'AssignmentToGo', label: 'In Action', icon: 'bi-lightning-fill' },
    { value: 'Closed', label: 'Resolved', icon: 'bi-check-circle-fill' },
  ];

  // Update state
  updatingId: string | null = null;

  // Delete
  deleteTarget: Report | null = null;
  isDeleting = false;

  // Toast
  toast = { visible: false, message: '', isError: false };
  private toastTimer: any;

  constructor(private http: HttpClient, private router: Router) { }

  ngOnInit(): void {
    const token = localStorage.getItem('authToken');
    if (!token) {
      this.router.navigate(['/login']);
      return;
    }
    this.loadReports();
  }

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('authToken') ?? '';
    return new HttpHeaders({ Authorization: `Bearer ${token}` });
  }

  loadReports(): void {
    this.isLoading = true;
    this.loadError = '';
    this.http.get<Report[]>(`${this.apiBase}/reports/all-reports`, { headers: this.getHeaders() })
      .subscribe({
        next: (data) => {
          this.reports = data.map(r => ({ ...r, _draftNotes: '' }));
          this.computeStats();
          this.applyFilters();
          this.isLoading = false;
        },
        error: (err) => {
          this.isLoading = false;
          if (err.status === 401) {
            localStorage.removeItem('authToken');
            this.router.navigate(['/login']);
          } else {
            this.loadError = 'Failed to load reports. Please try again.';
          }
        }
      });
  }

  computeStats(): void {
    this.stats = {
      total: this.reports.length,
      pending: this.reports.filter(r => r.status === 'Pending').length,
      underReview: this.reports.filter(r => r.status === 'UnderReview').length,
      inAction: this.reports.filter(r => r.status === 'AssignmentToGo').length,
      closed: this.reports.filter(r => r.status === 'Closed').length,
    };
  }

  applyFilters(): void {
    let result = [...this.reports];

    if (this.statusFilter) {
      result = result.filter(r => r.status === this.statusFilter);
    }

    if (this.searchQuery.trim()) {
      const q = this.searchQuery.trim().toLowerCase();
      result = result.filter(r =>
        r.title.toLowerCase().includes(q) ||
        r.anonymousTrackingCode.toLowerCase().includes(q) ||
        (r.incidentType ?? '').toLowerCase().includes(q)
      );
    }

    // Sort
    result.sort((a, b) => {
      const av = (a as any)[this.sortField] ?? '';
      const bv = (b as any)[this.sortField] ?? '';
      return this.sortAsc ? av.localeCompare(bv) : bv.localeCompare(av);
    });

    this.filteredReports = result;
  }

  setFilter(status: string): void {
    this.statusFilter = status;
    this.applyFilters();
  }

  sortBy(field: string): void {
    if (this.sortField === field) {
      this.sortAsc = !this.sortAsc;
    } else {
      this.sortField = field;
      this.sortAsc = false;
    }
    this.applyFilters();
  }

  toggleExpand(id: string): void {
    this.expandedId = this.expandedId === id ? null : id;
  }

  toggleSidebar(): void {
    this.sidebarCollapsed = !this.sidebarCollapsed;
  }

  // ── Update Status ────────────────────────────────────────
  updateStatus(report: Report, newStatus: string): void {
    if (report.status === newStatus || this.updatingId) return;
    this.updatingId = report.id;

    this.http.put(
      `${this.apiBase}/reports/${report.id}/status`,
      JSON.stringify(newStatus),
      { headers: this.getHeaders().set('Content-Type', 'application/json') }
    ).subscribe({
      next: () => {
        report.status = newStatus;
        report.updatedAt = new Date().toISOString();
        this.computeStats();
        this.applyFilters();
        this.updatingId = null;
        this.showToast('Status updated successfully.');
      },
      error: () => {
        this.updatingId = null;
        this.showToast('Failed to update status.', true);
      }
    });
  }

  // ── Save Admin Notes ─────────────────────────────────────
  saveNotes(report: Report): void {
    if (!report._draftNotes?.trim() || this.updatingId) return;
    this.updatingId = report.id;

    this.http.put(
      `${this.apiBase}/reports/${report.id}/notes`,
      JSON.stringify(report._draftNotes.trim()),
      { headers: this.getHeaders().set('Content-Type', 'application/json') }
    ).subscribe({
      next: () => {
        report.adminNotes = report._draftNotes!.trim();
        report._draftNotes = '';
        report.updatedAt = new Date().toISOString();
        this.updatingId = null;
        this.showToast('Admin notes saved.');
      },
      error: () => {
        this.updatingId = null;
        this.showToast('Failed to save notes.', true);
      }
    });
  }

  // ── Delete ───────────────────────────────────────────────
  confirmDelete(report: Report): void {
    this.deleteTarget = report;
  }

  cancelDelete(): void {
    this.deleteTarget = null;
  }

  deleteReport(): void {
    if (!this.deleteTarget) return;
    this.isDeleting = true;
    const id = this.deleteTarget.id;

    this.http.delete(`${this.apiBase}/reports/${id}`, { headers: this.getHeaders() })
      .subscribe({
        next: () => {
          this.reports = this.reports.filter(r => r.id !== id);
          this.computeStats();
          this.applyFilters();
          if (this.expandedId === id) this.expandedId = null;
          this.isDeleting = false;
          this.deleteTarget = null;
          this.showToast('Report deleted successfully.');
        },
        error: () => {
          this.isDeleting = false;
          this.showToast('Failed to delete report.', true);
        }
      });
  }

  // ── Helpers ──────────────────────────────────────────────
  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      'Pending': 'badge-pending',
      'UnderReview': 'badge-review',
      'AssignmentToGo': 'badge-action',
      'Closed': 'badge-closed',
    };
    return map[status] ?? 'badge-pending';
  }

  getStatusDisplay(status: string): string {
    const map: Record<string, string> = {
      'Pending': 'Pending',
      'UnderReview': 'Under Review',
      'AssignmentToGo': 'In Action',
      'Closed': 'Resolved',
    };
    return map[status] ?? status;
  }

  showToast(message: string, isError = false): void {
    clearTimeout(this.toastTimer);
    this.toast = { visible: true, message, isError };
    this.toastTimer = setTimeout(() => this.toast.visible = false, 3200);
  }

  logout(): void {
    localStorage.removeItem('authToken');
    this.router.navigate(['/login']);
  }
}
