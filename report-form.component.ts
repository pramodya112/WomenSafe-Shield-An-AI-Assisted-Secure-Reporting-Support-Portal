import { Component, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
    selector: 'app-report',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, RouterModule],
    templateUrl: './report-form.component.html',
    styleUrls: ['./report-form.component.css']
})
export class ReportFormComponent {

    @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

    reportForm = new FormGroup({
        reporterType: new FormControl<'victim' | 'behalf'>('victim'),
        incidentTypes: new FormControl<string[]>([]),
        severity: new FormControl<'low' | 'moderate' | 'high'>('moderate'),
        description: new FormControl('', Validators.required),
        incidentDate: new FormControl(''),
        incidentTime: new FormControl(''),
        location: new FormControl(''),
        nameOrAlias: new FormControl(''),
        contactEmail: new FormControl('', Validators.email),
        additionalNotes: new FormControl(''),
    });

    isLoading = false;
    mobileMenuOpen = false;
    errorMessage = '';
    successMessage = '';
    trackingCode = '';

    // Files selected by the user
    selectedFiles: File[] = [];
    uploadedFileNames: string[] = [];

    readonly incidentTypeOptions = [
        'Harassment', 'Physical Abuse', 'Sexual Violence',
        'Emotional Abuse', 'Stalking', 'Cyber Harassment',
        'Workplace', 'Other'
    ];

    private readonly baseUrl = 'http://localhost:5032/api/Reports';

    constructor(private http: HttpClient) { }

    /* ── Reporter type ── */
    setReporterType(type: 'victim' | 'behalf'): void {
        this.reportForm.patchValue({ reporterType: type });
    }
    isReporterType(type: 'victim' | 'behalf'): boolean {
        return this.reportForm.get('reporterType')?.value === type;
    }

    /* ── Incident type chips ── */
    toggleIncidentType(type: string): void {
        const ctrl = this.reportForm.get('incidentTypes')!;
        const current = ctrl.value ?? [];
        ctrl.setValue(
            current.includes(type) ? current.filter(t => t !== type) : [...current, type]
        );
    }
    isIncidentTypeSelected(type: string): boolean {
        return (this.reportForm.get('incidentTypes')?.value ?? []).includes(type);
    }

    /* ── Severity ── */
    setSeverity(level: 'low' | 'moderate' | 'high'): void {
        this.reportForm.patchValue({ severity: level });
    }
    isSeverity(level: 'low' | 'moderate' | 'high'): boolean {
        return this.reportForm.get('severity')?.value === level;
    }

    /* ── File selection ── */
    onFilesSelected(event: Event): void {
        const input = event.target as HTMLInputElement;
        if (input.files && input.files.length > 0) {
            this.selectedFiles = Array.from(input.files);
        }
    }

    removeFile(index: number): void {
        this.selectedFiles.splice(index, 1);
    }

    formatFileSize(bytes: number): string {
        if (bytes < 1024) return `${bytes} B`;
        if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
        return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
    }

    /* ── Submit: Step 1 → JSON, Step 2 → Files ── */
    onSubmit(): void {
        if (this.reportForm.invalid) return;

        this.isLoading = true;
        this.errorMessage = '';
        this.successMessage = '';
        this.trackingCode = '';

        const v = this.reportForm.value;

        const payload = {
            title: v.incidentTypes?.join(', ') || 'Incident Report',
            description: v.description,
            location: v.location,
            incidentType: v.incidentTypes?.join(', ') || 'General',
            contactInfo: v.contactEmail,
            victimName: v.nameOrAlias,
            reporterType: v.reporterType,
            incidentDate: v.incidentDate,
            incidentTime: v.incidentTime,
            severity: v.severity,
            additionalNotes: v.additionalNotes,
        };

        // ── Step 1: Submit the report JSON ──
        this.http.post<{ message: string; reportId: string; trackingCode: string }>(
            `${this.baseUrl}/submit`, payload
        ).subscribe({
            next: (res) => {
                this.trackingCode = res.trackingCode;

                // ── Step 2: Upload evidence files if any were selected ──
                if (this.selectedFiles.length > 0) {
                    this.uploadEvidence(res.reportId);
                } else {
                    this.onSuccess();
                }
            },
            error: (err) => {
                this.isLoading = false;
                this.errorMessage = err.error?.message || err.error || 'Submission failed. Please try again.';
            }
        });
    }

    private uploadEvidence(reportId: string): void {
        const formData = new FormData();
        this.selectedFiles.forEach(file => formData.append('files', file, file.name));

        this.http.post<{ message: string; files: any[] }>(
            `${this.baseUrl}/${reportId}/evidence`, formData
        ).subscribe({
            next: (res) => {
                this.uploadedFileNames = res.files.map(f => f.fileName);
                this.onSuccess();
            },
            error: (err) => {
                // Report was saved — only the file upload failed
                this.isLoading = false;
                this.successMessage =
                    `Report saved (tracking code: ${this.trackingCode}), ` +
                    `but file upload failed: ${err.error?.message ?? 'Unknown error'}. ` +
                    `You can re-upload evidence later using your tracking code.`;
            }
        });
    }

    private onSuccess(): void {
        this.isLoading = false;
        this.successMessage =
            `Your report has been submitted securely. ` +
            `Your private tracking code is: ${this.trackingCode}` +
            (this.uploadedFileNames.length
                ? ` · ${this.uploadedFileNames.length} evidence file(s) saved.`
                : '');

        this.selectedFiles = [];
        this.reportForm.reset({
            reporterType: 'victim',
            severity: 'moderate',
            incidentTypes: [],
        });
    }
}