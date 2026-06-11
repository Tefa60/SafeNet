// SafeNet.Web/Models/ViewModels/AdminViewModel.cs
using SafeNet.Data.Entidades;

namespace SafeNet.Web.Models.ViewModels
{
    public class AdminViewModel
    {
        public int TotalAnalyses  { get; set; }
        public int TotalScams     { get; set; }
        public int TotalSuspect   { get; set; }
        public int TotalSafe      { get; set; }
        public int TotalUsers     { get; set; }
        public List<AnalysisEntity> RecentAnalyses { get; set; } = new();
    }
}
