namespace SafeNet.Web.Models.ViewModels
{
    public class AnalysisResultVM
    {
        public string Veredicto { get; set; } = string.Empty;
        public string NivelRiesgo { get; set; } = string.Empty;
        public string Explicacion { get; set; } = string.Empty;
        public string TextoAnalizado { get; set; } = string.Empty;
        public DateTime FechaAnalisis { get; set; }
        public bool EsSimulacion { get; set; }

        public string ColorVeredicto => Veredicto switch
        {
            "ESTAFA" => "danger",
            "SOSPECHOSO" => "warning",
            "LEGITIMO" => "success",
            _ => "secondary"
        };

        public string IconoVeredicto => Veredicto switch
        {
            "ESTAFA" => "bi-x-circle-fill",
            "SOSPECHOSO" => "bi-exclamation-triangle-fill",
            "LEGITIMO" => "bi-check-circle-fill",
            _ => "bi-question-circle"
        };
    }
}