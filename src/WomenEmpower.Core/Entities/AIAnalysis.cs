using System;
namespace WomenEmpower.Core.Entities
{
    public class AIAnalysis
    {
        public int id { get; set; }
        public Guid ReportId { get; set; }

        public double AbuseConfidenceScore { get; set; }

        public string SentimentLabel { get; set; }

        public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;

        public Report Report { get; set; }

    }
}

