namespace WomenEmpower.Core.Entities
{
    public class Evidence
    {
        public int Id { get; set; }
        public Guid ReportId { get; set; }
        public string FilePath { get; set; }

        public string FileType { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public Report Report { get; set; }
    }
}
