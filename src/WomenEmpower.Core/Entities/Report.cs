using System;
using System.Collections.Generic;

namespace WomenEmpower.Core.Entities
{
    public class Report
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string AnonymousTrackingCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }

        // Admin fields
        public DateTimeOffset? UpdatedAt { get; set; }
        public string? AdminNotes { get; set; }

        public ReportStatus Status { get; set; } = ReportStatus.Pending;

        public string Title { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string IncidentType { get; set; } = string.Empty;
        public string? ContactInfo { get; set; }
        public string? VictimName { get; set; }
        public int? VictimAge { get; set; }
        public string? ReporterType { get; set; }
        public string? Relationship { get; set; }

        // Stored as strings — avoids DateOnly/TimeOnly SQL Server compat issues
        public string? IncidentDate { get; set; }   // "yyyy-MM-dd"
        public string? IncidentTime { get; set; }   // "HH:mm"

        public string? Severity { get; set; }   // "low" | "moderate" | "high"
        public string? AdditionalNotes { get; set; }

        // ── Navigation properties ─────────────────────────────────────────
        public ICollection<Evidence> Evidence { get; set; } = new List<Evidence>();
        public AIAnalysis? Analysis { get; set; }
    }

    public enum ReportStatus
    {
        Pending,
        UnderReview,
        AssignmentToGo,
        Closed
    }
}