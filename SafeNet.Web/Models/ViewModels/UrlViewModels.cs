// SafeNet.Web/Models/ViewModels/UrlViewModels.cs
using System.ComponentModel.DataAnnotations;

namespace SafeNet.Web.Models.ViewModels
{
    public class UrlInputViewModel
    {
        [Required(ErrorMessage = "Ingresa una URL.")]
        [Display(Name = "URL a verificar")]
        public string Url { get; set; } = string.Empty;
    }

    public class UrlResultViewModel
    {
        public string       Url            { get; set; } = string.Empty;
        public string       Verdict        { get; set; } = string.Empty;
        public int          RiskScore      { get; set; }
        public List<string> Signals        { get; set; } = new();
        public string       Recommendation { get; set; } = string.Empty;
        public bool         BlacklistHit   { get; set; }
    }
}
