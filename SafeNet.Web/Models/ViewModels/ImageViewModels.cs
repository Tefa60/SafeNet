// SafeNet.Web/Models/ViewModels/ImageViewModels.cs
using System.ComponentModel.DataAnnotations;

namespace SafeNet.Web.Models.ViewModels
{
    public class ImageInputViewModel
    {
        [Required(ErrorMessage = "Selecciona una imagen.")]
        [Display(Name = "Imagen a analizar")]
        public IFormFile? ImageFile { get; set; }
    }

    public class ImageResultViewModel
    {
        public string       ExtractedText  { get; set; } = string.Empty;
        public string       Verdict        { get; set; } = string.Empty;
        public int          RiskScore      { get; set; }
        public List<string> Signals        { get; set; } = new();
        public string       Recommendation { get; set; } = string.Empty;
    }
}
