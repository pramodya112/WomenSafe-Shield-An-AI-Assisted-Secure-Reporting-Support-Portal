using System;
using System.Collections.Generic;

namespace WomenEmpower.Core.Entities
{
    public class Report
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string AnonymousTrackingCode { get; set; }

        public string Description { get; set; }
        public string Category { get; set; }

        public DateTime CreatedAt { get; set; }
        
        public ReportStatus Status { get; set; } = ReportStatus.Pending;
        public ICollection<Evidence> Evidence { get; set; }
        public AIAnalysis Analysis { get; set; }
    }

    public enum ReportStatus
    {
        Pending,
        UnderReview,
        AssignmentToGo,
        Closed
    }

}
