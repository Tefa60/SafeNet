using SafeNet.Data.Entidades;

namespace SafeNet.Web.Models.ViewModels
{
    public class HistoryViewModel
    {
        public List<AnalysisEntity> Analyses { get; set; } = new();
        public AnalysisEntity? SelectedAnalysis { get; set; }
        public string? FilterVerdict { get; set; }
        public string? FilterType { get; set; }
        public DateTime? FilterFrom { get; set; }
        public DateTime? FilterTo { get; set; }
    }
}