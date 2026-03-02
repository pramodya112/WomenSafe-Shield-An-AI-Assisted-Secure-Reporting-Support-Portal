import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { trigger, transition, style, animate } from '@angular/animations';

interface ProfileResult {
  platform: string;
  username: string;
  riskLevel: 'Low' | 'Medium' | 'High' | 'Critical';
  riskScore: number;
  explanation: string;
  redFlags: string[];
  recommendedAction: string;
  actionDetail: string;
}

@Component({
  selector: 'app-fake-profile-checker',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './fake-profile-checker.component.html',
  styleUrls: ['./fake-profile-checker.component.css'],
  animations: [
    trigger('slideIn', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(20px)' }),
        animate('400ms ease-out', style({ opacity: 1, transform: 'translateY(0)' }))
      ])
    ])
  ]
})
export class FakeProfileCheckerComponent {

  profileUrl = '';
  isAnalyzing = false;
  errorMsg = '';
  result: ProfileResult | null = null;

  // ── Points to YOUR backend — which then calls Anthropic securely ──
  private readonly apiUrl = 'http://localhost:5032/api/FakeProfile/analyze';

  constructor(private http: HttpClient) { }

  canSubmit(): boolean {
    return this.profileUrl.trim().length > 10;
  }

  analyzeProfile(): void {
    if (!this.canSubmit() || this.isAnalyzing) return;

    this.isAnalyzing = true;
    this.errorMsg = '';
    this.result = null;

    this.http.post<ProfileResult>(this.apiUrl, { profileUrl: this.profileUrl.trim() })
      .subscribe({
        next: (data) => {
          this.result = data;
          this.isAnalyzing = false;
        },
        error: (err) => {
          this.isAnalyzing = false;
          this.errorMsg =
            err.error?.message ||
            err.error ||
            'Analysis failed. Please try again.';
        }
      });
  }

  reset(): void {
    this.profileUrl = '';
    this.result = null;
    this.errorMsg = '';
    this.isAnalyzing = false;
  }

  getRiskIcon(): string {
    const map: Record<string, string> = {
      Low: 'bi bi-check-circle-fill',
      Medium: 'bi bi-exclamation-circle-fill',
      High: 'bi bi-exclamation-triangle-fill',
      Critical: 'bi bi-x-octagon-fill',
    };
    return map[this.result?.riskLevel ?? 'Low'];
  }

  getRiskColor(): string {
    const map: Record<string, string> = {
      Low: '#22c55e',
      Medium: '#f59e0b',
      High: '#ef4444',
      Critical: '#7c3aed',
    };
    return map[this.result?.riskLevel ?? 'Low'];
  }

  getScoreOffset(): number {
    // strokeDasharray = 125.6  (2π × 20)
    const score = this.result?.riskScore ?? 0;
    return 125.6 - (score / 100) * 125.6;
  }

  getActionIcon(): string {
    const map: Record<string, string> = {
      Low: 'bi bi-check-circle-fill',
      Medium: 'bi bi-eye-fill',
      High: 'bi bi-shield-exclamation',
      Critical: 'bi bi-shield-x',
    };
    return map[this.result?.riskLevel ?? 'Low'];
  }
}
