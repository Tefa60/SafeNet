// SafeNet.Core/Services/UrlCheckerService.cs
using SafeNet.Core.Interfaces;
using SafeNet.Data.Entidades;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SafeNet.Core.Services
{
    public class UrlCheckerResult
    {
        public string Verdict       { get; set; } = "SEGURA";
        public int    RiskScore     { get; set; } = 0;
        public List<string> Signals { get; set; } = new();
        public string Recommendation { get; set; } = string.Empty;
        public bool   BlacklistHit  { get; set; } = false;
    }

    public class UrlCheckerService
    {
        private readonly ClaudeApiService  _claude;
        private readonly IAnalysisService  _analysis;

        private static readonly HashSet<string> Blacklist = new(StringComparer.OrdinalIgnoreCase)
        {
            "phishing-site.com", "paypal-verification.xyz", "bcp-verificacion.net",
            "interbank-seguro.xyz", "bbva-alerta.net", "scotiabank-verify.com",
            "amazon-prize.xyz", "win-gift-card.net", "free-iphone-winner.com"
        };

        private static readonly HashSet<string> ShortenerDomains = new(StringComparer.OrdinalIgnoreCase)
        {
            "bit.ly", "tinyurl.com", "goo.gl", "t.co", "ow.ly", "short.link"
        };

        public UrlCheckerService(ClaudeApiService claude, IAnalysisService analysis)
        {
            _claude   = claude;
            _analysis = analysis;
        }

        public async Task<UrlCheckerResult> CheckUrlAsync(string url, string userId)
        {
            var result = new UrlCheckerResult();

            // ── 1. Validar formato ──────────────────────────────────────────
            if (string.IsNullOrWhiteSpace(url) ||
                (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                 !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
            {
                result.Verdict    = "INVÁLIDA";
                result.RiskScore  = 0;
                result.Recommendation = "La URL ingresada no tiene un formato válido. Debe comenzar con http:// o https://";
                await GuardarAsync(url, userId, result);
                return result;
            }

            Uri uri;
            try { uri = new Uri(url); }
            catch
            {
                result.Verdict   = "INVÁLIDA";
                result.RiskScore = 0;
                result.Recommendation = "No se pudo parsear la URL.";
                await GuardarAsync(url, userId, result);
                return result;
            }

            string domain = uri.Host.ToLower().Replace("www.", "");

            // ── 2. Lista negra ─────────────────────────────────────────────
            if (Blacklist.Contains(domain))
            {
                result.Verdict        = "ESTAFA";
                result.RiskScore      = 95;
                result.BlacklistHit   = true;
                result.Signals        = new List<string> { "Dominio en lista negra de phishing conocido" };
                result.Recommendation = "Este dominio está registrado como sitio de phishing. No accedas bajo ninguna circunstancia.";
                await GuardarAsync(url, userId, result);
                return result;
            }

            // ── 3. Señales locales ──────────────────────────────────────────
            var signals = new List<string>();
            int score   = 0;

            // IP en lugar de dominio
            if (Regex.IsMatch(domain, @"^\d{1,3}(\.\d{1,3}){3}$"))
            {
                signals.Add("Usa dirección IP en lugar de dominio");
                score += 30;
            }

            // URL acortada
            if (ShortenerDomains.Contains(domain))
            {
                signals.Add("URL acortada — destino desconocido");
                score += 20;
            }

            // Símbolo @
            if (url.Contains('@'))
            {
                signals.Add("Contiene símbolo @ — técnica de engaño común");
                score += 25;
            }

            // Subdominio excesivo
            if (uri.Host.Split('.').Length > 4)
            {
                signals.Add("Demasiados subdominios — patrón sospechoso");
                score += 15;
            }

            // Palabras clave de phishing en el dominio
            string[] phishWords = { "verify", "secure", "login", "update", "bank", "account", "confirm" };
            foreach (var word in phishWords)
            {
                if (domain.Contains(word))
                {
                    signals.Add($"Palabra sospechosa en el dominio: '{word}'");
                    score += 10;
                    break;
                }
            }

            result.Signals   = signals;
            result.RiskScore = Math.Min(score, 89); // Máx 89 para no-blacklist

            // ── 4. Consultar Claude ─────────────────────────────────────────
            try
            {
                string prompt   = $"Analiza esta URL en busca de señales de phishing o estafa: {url}";
                string response = await _claude.AnalizarMensajeAsync(prompt);

                var json = JsonSerializer.Deserialize<JsonElement>(response);
                string verdict = json.GetProperty("verdict").GetString() ?? "SEGURA";
                int aiScore = (int)json.GetProperty("riskScore").GetDouble();

                result.Verdict    = verdict;
                result.RiskScore  = Math.Max(result.RiskScore, aiScore);
                result.Recommendation = json.GetProperty("recommendation").GetString() ?? "";

                if (json.TryGetProperty("signals", out var aiSignals))
                {
                    foreach (var s in aiSignals.EnumerateArray())
                    {
                        string? sig = s.GetString();
                        if (sig != null && !result.Signals.Contains(sig))
                            result.Signals.Add(sig);
                    }
                }
            }
            catch
            {
                // Fallback local si Claude falla
                result.Verdict = result.RiskScore switch
                {
                    >= 70 => "ESTAFA",
                    >= 40 => "SOSPECHOSA",
                    _     => "SEGURA"
                };
                result.Recommendation = result.Verdict switch
                {
                    "ESTAFA"     => "Esta URL tiene múltiples señales de peligro. No la visites.",
                    "SOSPECHOSA" => "Procede con precaución. Verifica la autenticidad del sitio.",
                    _            => "No se detectaron señales de alerta evidentes."
                };
            }

            await GuardarAsync(url, userId, result);
            return result;
        }

        private async Task GuardarAsync(string url, string userId, UrlCheckerResult result)
        {
            await _analysis.AnalizarTextoAsync(url, userId);
        }
    }
}

