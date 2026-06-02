namespace SafeNet.Core.Models
{
    public class AnalysisResult
    {
        public string Veredicto { get; set; } = string.Empty;
        public string NivelRiesgo { get; set; } = string.Empty;
        public string Explicacion { get; set; } = string.Empty;
        public DateTime FechaAnalisis { get; set; } = DateTime.Now;
        public string TextoAnalizado { get; set; } = string.Empty;
        public bool EsSimulacion { get; set; } = true;
    }
}