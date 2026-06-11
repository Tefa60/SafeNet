// SafeNet.Web/Models/ViewModels/HomeViewModel.cs
namespace SafeNet.Web.Models.ViewModels
{
    public class HomeViewModel
    {
        public int TotalAnalyses { get; set; }
        public int TotalScams    { get; set; }
        public int TotalSafe     { get; set; }
        public int UserAnalyses  { get; set; }
        public int UserScams     { get; set; }
    }
}
