import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { ReportFormComponent } from './components/report-form/report-form.component';
import { DonateComponent } from './components/donate/donate.component';
import { NewsComponent } from './components/news/news.component';
import { NewsDetailComponent } from './components/news-detail/news-detail.component';
import { TrackReportComponent } from './components/track-report/track-report.component';
import { AdminDashboardComponent } from './components/admin/admin-dashboard.component';
import { FakeProfileCheckerComponent } from './components/fake-profile-checker/fake-profile-checker.component';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'home', component: HomeComponent },
  { path: 'report', component: ReportFormComponent },
  { path: 'track', component: TrackReportComponent },
  { path: 'admin', component: AdminDashboardComponent },
  { path: 'profile-checker', component: FakeProfileCheckerComponent },
  { path: 'donate', component: DonateComponent },
  { path: 'news', component: NewsComponent },
  { path: 'news-detail', component: NewsDetailComponent },
];
