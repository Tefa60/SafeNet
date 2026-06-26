// SafeNet.Web/Controllers/ImageController.cs
using Microsoft.AspNetCore.Mvc;
using SafeNet.Core.Interfaces;
using SafeNet.Core.Services;
using SafeNet.Data;
using SafeNet.Data.Entidades;
using SafeNet.Web.Models.ViewModels;
using System.Security.Claims;
using System.Text.Json;

namespace SafeNet.Web.Controllers
{
    public class ImageController : Controller
    {
        private readonly OcrService _ocr;
        private readonly ClaudeApiService _claude;
        private readonly IAnalysisService _analysis;
        private readonly AppDbContext _context;

        public ImageController(OcrService ocr, ClaudeApiService claude, IAnalysisService analysis, AppDbContext context)
        {
            _ocr = ocr;
            _claude = claude;
            _analysis = analysis;
            _context = context;
        }

        public IActionResult Index() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Analyze(ImageInputViewModel model)
        {
            if (!ModelState.IsValid || model.ImageFile == null)
                return View("Index", model);

            // Leer bytes de la imagen
            byte[] imageBytes;
            using (var ms = new MemoryStream())
            {
                await model.ImageFile.CopyToAsync(ms);
                imageBytes = ms.ToArray();
            }

            // Extraer texto con OCR
            string extractedText = _ocr.ExtractText(imageBytes);

            if (string.IsNullOrWhiteSpace(extractedText) || extractedText.StartsWith("[OCR"))
            {
                extractedText = "[No se pudo extraer texto de la imagen]";
            }

            // Analizar con Claude
            string verdict        = "SEGURA";
            int    riskScore      = 0;
            string recommendation = "No se detectaron senales de alerta.";
            var    signals        = new List<string>();

            try
            {
                string prompt    = $"Analiza el siguiente texto extraido de una imagen en busca de senales de estafa o phishing: {extractedText}";
                string response  = await _claude.AnalizarMensajeAsync(prompt);
                var    json       = JsonSerializer.Deserialize<JsonElement>(response);

                verdict        = json.GetProperty("verdict").GetString()        ?? "SEGURA";
                riskScore      = json.GetProperty("riskScore").GetInt32();
                recommendation = json.GetProperty("recommendation").GetString() ?? "";

                if (json.TryGetProperty("signals", out var sigs))
                    foreach (var s in sigs.EnumerateArray())
                        signals.Add(s.GetString() ?? "");
            }
            catch { /* fallback: valores por defecto */ }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonimo";

            // --- NUEVO: persistencia del analisis en la tabla Analyses ---
            var entity = new AnalysisEntity
            {
                UserId         = userId,
                TypeId         = 3, // 3 = "Imagen" segun seed data de AnalysisTypeEntity
                InputContent   = extractedText,
                Verdict        = verdict,
                RiskScore      = (byte)riskScore,
                Signals        = string.Join(", ", signals),
                Recommendation = recommendation,
                ImagePath      = model.ImageFile?.FileName
            };

            _context.Analyses.Add(entity);
            await _context.SaveChangesAsync();
            // --- FIN NUEVO ---

            var vm = new ImageResultViewModel
            {
                ExtractedText  = extractedText,
                Verdict        = verdict,
                RiskScore      = riskScore,
                Signals        = signals,
                Recommendation = recommendation
            };

            return View("Result", vm);
        }
    }
}