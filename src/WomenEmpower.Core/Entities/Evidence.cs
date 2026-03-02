using System;

namespace WomenEmpower.Core.Entities
{
    public class Evidence
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // FK — matches Report.Id (int)
        public Guid ReportId { get; set; }
        public Report Report { get; set; } = null!;

        public string FileName { get; set; } = string.Empty;  // original name shown to user
        public string StoredName { get; set; } = string.Empty;  // GUID-based name on disk
        public string FilePath { get; set; } = string.Empty;  // e.g. /uploads/evidence/42/abc.jpg
        public long FileSize { get; set; }                   // bytes
        public string ContentType { get; set; } = string.Empty;  // MIME type

        public DateTimeOffset UploadedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}