using System.ComponentModel.DataAnnotations;

namespace SafeNet.Data.Entidades
{
    public class ReportedUrlEntity
    {
        [Key]
        public int UrlId { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Url { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Domain { get; set; }

        [MaxLength(20)]
        public string? RiskLevel { get; set; }

        public string? ReportedBy { get; set; }

        public int ReportCount { get; set; } = 1;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}