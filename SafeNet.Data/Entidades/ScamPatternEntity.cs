using System.ComponentModel.DataAnnotations;

namespace SafeNet.Data.Entidades
{
    public class ScamPatternEntity
    {
        [Key]
        public int PatternId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Keyword { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Category { get; set; }

        public byte RiskWeight { get; set; } = 50;

        public bool IsActive { get; set; } = true;

        public string? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}