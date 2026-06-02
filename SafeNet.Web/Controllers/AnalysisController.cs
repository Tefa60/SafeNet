using Microsoft.AspNetCore.Mvc;
using SafeNet.Core.Interfaces;
using SafeNet.Web.Models.ViewModels;

namespace SafeNet.Web.Controllers
{
    public class AnalysisController : Controller
    {
        private readonly IAnalysisService _analysisService;

        public AnalysisController(IAnalysisService analysisService)
        {
            _analysisService = analysisService;
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

            return View("Result", resultVM);
        }
    }
}