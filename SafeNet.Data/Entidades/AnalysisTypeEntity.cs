using System.ComponentModel.DataAnnotations;

namespace SafeNet.Data.Entidades
{
    public class AnalysisTypeEntity
    {
        [Key]
        public int TypeId { get; set; }

        [Required]
        [MaxLength(50)]
        public string TypeName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<AnalysisEntity> Analyses { get; set; }
            = new List<AnalysisEntity>();
    }
}