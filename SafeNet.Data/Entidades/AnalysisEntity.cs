using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeNet.Data.Entidades
{
    public class AnalysisEntity
    {
        [Key]
        public int AnalysisId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int TypeId { get; set; }

        [Required]
        public string InputContent { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Verdict { get; set; } = string.Empty;

        [Range(0, 100)]
        public byte RiskScore { get; set; }

        public string? Signals { get; set; }

        public string? Recommendation { get; set; }

        public string? ImagePath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("TypeId")]
        public AnalysisTypeEntity? AnalysisType { get; set; }
    }
}


