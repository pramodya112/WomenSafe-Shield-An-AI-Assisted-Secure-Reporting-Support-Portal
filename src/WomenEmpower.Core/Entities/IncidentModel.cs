using System.ComponentModel.DataAnnotations;

namespace WomenEmpower.Core.Entities
{
    public class IncidentModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Type { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public DateTime SubmittedAt { get; set; } = DateTime.Now;
    }
}