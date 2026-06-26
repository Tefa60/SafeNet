using Microsoft.AspNetCore.Mvc;
using SafeNet.Core.Interfaces;
using SafeNet.Data;
using SafeNet.Data.Entidades;
using SafeNet.Web.Models.ViewModels;

namespace SafeNet.Web.Controllers
{
    public class AnalysisController : Controller
    {
        private readonly IAnalysisService _analysisService;
        private readonly AppDbContext _context;

        public AnalysisController(IAnalysisService analysisService, AppDbContext context)
        {
            _analysisService = analysisService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View(new AnalysisInputVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Analyze(AnalysisInputVM model)
        {
            if (!ModelState.IsValid)
                return View("Index", model);

            var usuarioId = User.Identity?.Name ?? "anonimo";
            var resultado = await _analysisService.AnalizarTextoAsync(model.Texto, usuarioId);

            var resultVM = new AnalysisResultVM
            {
                Veredicto = resultado.Veredicto,
                NivelRiesgo = resultado.NivelRiesgo,
                Explicacion = resultado.Explicacion,
                TextoAnalizado = resultado.TextoAnalizado,
                FechaAnalisis = resultado.FechaAnalisis,
                EsSimulacion = resultado.EsSimulacion
            };

            // --- NUEVO: persistencia del analisis en la tabla Analyses ---
            byte riskScore = resultado.NivelRiesgo?.Trim().ToUpperInvariant() switch
            {
                "BAJO" => 20,
                "MEDIO" => 50,
                "ALTO" => 80,
                "CRITICO" or "CRÍTICO" => 95,
                _ => 50 // valor por defecto si viene un texto inesperado
            };

            var entity = new AnalysisEntity
            {
                UserId         = usuarioId,
                TypeId         = 1, // 1 = "Texto" segun seed data de AnalysisTypeEntity
                InputContent   = resultado.TextoAnalizado,
                Verdict        = resultado.Veredicto,
                RiskScore      = riskScore,
                Signals        = resultado.NivelRiesgo, // se guarda el nivel original como texto
                Recommendation = resultado.Explicacion
            };

            _context.Analyses.Add(entity);
            await _context.SaveChangesAsync();
            // --- FIN NUEVO ---

            return View("Result", resultVM);
        }
    }
}