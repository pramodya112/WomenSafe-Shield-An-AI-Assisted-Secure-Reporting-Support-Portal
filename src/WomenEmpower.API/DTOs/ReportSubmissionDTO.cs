namespace WomenEmpower.API.DTOs
{
    public class ReportSubmissionDTO
    {
        public string Title { get; set; } =string.Empty;
        public string Description { get; set; } =string.Empty;
        public string Location { get; set; } = string.Empty;
        public string IncidentType { get; set; } ="General";

        public string Category { get; set; } = string.Empty;
        public string? ContactInfo { get; set; } 
    }
}
