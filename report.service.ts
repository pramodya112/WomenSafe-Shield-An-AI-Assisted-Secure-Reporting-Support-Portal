import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Report } from '../models/report.model';

@Injectable({
    providedIn: 'root'
})
export class ReportService {
    private apiUrl = 'http://localhost:7063/api/Reports';

    constructor(private http: HttpClient) { }

    submitReport(report: Report): Observable<any> {
        return this.http.post(this.apiUrl + '/submit', report);
    }

    getAllReports(status?: string): Observable<Report[]> {
        let url = this.apiUrl + '/all-reports';
        if (status) {
            url += '?status=' + status;
        }
        return this.http.get<Report[]>(url);
    }
}