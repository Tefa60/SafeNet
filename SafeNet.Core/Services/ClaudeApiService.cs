using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace SafeNet.Core.Services
{
    public class ClaudeApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly bool _simulationMode;

        public ClaudeApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["ClaudeApi:ApiKey"] ?? "";
            _model = configuration["ClaudeApi:Model"] ?? "claude-sonnet-4-20250514";
            _simulationMode = _apiKey == "TU_API_KEY_AQUI" || string.IsNullOrEmpty(_apiKey);
        }

        public async Task<string> AnalizarMensajeAsync(string mensaje)
        {
            if (_simulationMode)
                return SimularAnalisis(mensaje);

            return await AnalizarConApiRealAsync(mensaje);
        }

        private string SimularAnalisis(string mensaje)
        {
            var mensajeLower = mensaje.ToLower();

            var palabrasEstafa = new[] {
                "ganaste", "premio", "transferir", "urgente", "clave", "pin",
                "banco", "verificar", "cuenta bloqueada", "click aqui", "gratis",
                "oferta", "ganador", "depositar", "billetera", "yape", "plin"
            };

            int puntaje = palabrasEstafa.Count(p => mensajeLower.Contains(p));

            string veredicto;
            string nivel;
            string explicacion;

            if (puntaje >= 3)
            {
                veredicto = "ESTAFA";
                nivel = "ALTO";
                explicacion = "Este mensaje contiene multiples indicadores tipicos de estafa digital. Se detectaron patrones de urgencia, solicitud de datos personales o promesas de premios.";
            }
            else if (puntaje >= 1)
            {
                veredicto = "SOSPECHOSO";
                nivel = "MEDIO";
                explicacion = "Este mensaje contiene algunos elementos que podrian indicar una estafa. Se recomienda verificar la fuente antes de tomar cualquier accion.";
            }
            else
            {
                veredicto = "LEGITIMO";
                nivel = "BAJO";
                explicacion = "No se detectaron patrones tipicos de estafa en este mensaje. Sin embargo, mantente siempre alerta ante mensajes inesperados.";
            }

            return $"{veredicto}|{nivel}|{explicacion}";
        }

        private async Task<string> AnalizarConApiRealAsync(string mensaje)
        {
            var prompt = $@"Eres un experto en ciberseguridad especializado en detectar estafas digitales en Peru.
Analiza el siguiente mensaje y determina si es una ESTAFA, SOSPECHOSO o LEGITIMO.

Mensaje a analizar:
{mensaje}

Responde EXACTAMENTE en este formato (sin nada mas):
VEREDICTO|NIVEL_RIESGO|EXPLICACION

Donde:
- VEREDICTO: solo puede ser ESTAFA, SOSPECHOSO o LEGITIMO
- NIVEL_RIESGO: solo puede ser ALTO, MEDIO o BAJO  
- EXPLICACION: una explicacion clara en espanol de maximo 2 oraciones

Ejemplo de respuesta:
ESTAFA|ALTO|Este mensaje solicita datos bancarios con urgencia, patron tipico de phishing. Nunca compartas tu PIN o clave con nadie.";

            var requestBody = new
            {
                model = _model,
                max_tokens = 300,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
            _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

            var response = await _httpClient.PostAsync("https://api.anthropic.com/v1/messages", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseBody);
            var texto = doc.RootElement
                .GetProperty("content")[0]
                .GetProperty("text")
                .GetString() ?? "SOSPECHOSO|MEDIO|No se pudo analizar el mensaje.";

            return texto.Trim();
        }
    }
}