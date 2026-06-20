using SafeNet.Core.Interfaces;
using SafeNet.Core.Models;
namespace SafeNet.Core.Services
{
    public class AnalysisService : IAnalysisService
    {
        private readonly ClaudeApiService _claudeApiService;
        public AnalysisService(ClaudeApiService claudeApiService)
        {
            _claudeApiService = claudeApiService;
        }
        public async Task<AnalysisResult> AnalizarTextoAsync(string texto, string usuarioId)
        {
            try
            {
                var respuesta = await _claudeApiService.AnalizarMensajeAsync(texto);
                var partes = respuesta.Split('|');
                var resultado = new AnalysisResult
                {
                    Veredicto = partes.Length > 0 ? partes[0].Trim() : "SOSPECHOSO",
                    NivelRiesgo = partes.Length > 1 ? partes[1].Trim() : "MEDIO",
                    Explicacion = partes.Length > 2 ? partes[2].Trim() : "No se pudo analizar el mensaje.",
                    TextoAnalizado = texto,
                    FechaAnalisis = DateTime.Now,
                    EsSimulacion = true
                };
                return resultado;
            }
            catch
            {
                // Fallback si Claude no está disponible (sin créditos, caída del servicio, etc.)
                return new AnalysisResult
                {
                    Veredicto = "SOSPECHOSO",
                    NivelRiesgo = "MEDIO",
                    Explicacion = "No se pudo completar el análisis con IA en este momento. Por precaución, verifica manualmente la autenticidad del mensaje antes de actuar.",
                    TextoAnalizado = texto,
                    FechaAnalisis = DateTime.Now,
                    EsSimulacion = true
                };
            }
        }
    }
}