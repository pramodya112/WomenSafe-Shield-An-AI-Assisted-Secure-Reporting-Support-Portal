export interface Report {
  id?: number;
  title: string;
  description: string;
  location: string;
  incidentType: string;
  category: string;
  contactInfo?: string;
  status?: string;
  anonymousTrackingCode?: string;
}
