using System.ComponentModel.DataAnnotations;

namespace SafeNet.Web.Models.ViewModels
{
    public class AnalysisInputVM
    {
        [Required(ErrorMessage = "Por favor ingresa un mensaje para analizar.")]
        [MinLength(10, ErrorMessage = "El mensaje debe tener al menos 10 caracteres.")]
        [Display(Name = "Mensaje o contenido sospechoso")]
        public string Texto { get; set; } = string.Empty;

        [Display(Name = "Tipo de análisis")]
        public string TipoAnalisis { get; set; } = "texto";
    }
}