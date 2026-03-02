namespace WomenEmpower.API.DTOs
{
    public class ReportSubmissionDTO
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? IncidentType { get; set; }
        public string? ContactInfo { get; set; }
        public string? VictimName { get; set; }
        public int? VictimAge { get; set; }
        public string? ReporterType { get; set; }   // "victim" | "behalf"
        public string? Relationship { get; set; }

        // ── New fields ──
        public string? IncidentDate { get; set; }   // "yyyy-MM-dd" from date input
        public string? IncidentTime { get; set; }   // "HH:mm"     from time input
        public string? Severity { get; set; }       // "low" | "moderate" | "high"
        public string? AdditionalNotes { get; set; }
    }
}