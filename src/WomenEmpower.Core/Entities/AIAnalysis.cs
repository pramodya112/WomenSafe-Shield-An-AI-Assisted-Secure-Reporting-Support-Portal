using System;

namespace WomenEmpower.Core.Entities
{
    public class AIAnalysis
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // FK — one-to-one with Report
        public Guid ReportId { get; set; }
        public Report Report { get; set; } = null!;

        public string? Summary { get; set; }   // AI-generated summary
        public string? RiskLevel { get; set; }   // "low" | "moderate" | "high" | "critical"
        public string? RecommendedAction { get; set; }   // e.g. "Escalate to police"
        public string? SentimentScore { get; set; }   // optional: "0.85 negative"
        public string? DetectedLanguage { get; set; }

        public DateTimeOffset AnalysedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}