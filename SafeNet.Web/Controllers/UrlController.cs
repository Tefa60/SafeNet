// SafeNet.Web/Controllers/UrlController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeNet.Core.Services;
using SafeNet.Data;
using SafeNet.Data.Entidades;
using SafeNet.Web.Models.ViewModels;
using System.Security.Claims;

namespace SafeNet.Web.Controllers
{
    public class UrlController : Controller
    {
        private readonly UrlCheckerService _urlChecker;
        private readonly AppDbContext _context;

        public UrlController(UrlCheckerService urlChecker, AppDbContext context)
        {
            _urlChecker = urlChecker;
            _context = context;
        }

        // GET: /Url
        public IActionResult Index() => View();

        // POST: /Url/Check
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Check(UrlInputViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Index", model);

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonimo";
            var result = await _urlChecker.CheckUrlAsync(model.Url, userId);

            var vm = new UrlResultViewModel
            {
                Url            = model.Url,
                Verdict        = result.Verdict,
                RiskScore      = result.RiskScore,
                Signals        = result.Signals,
                Recommendation = result.Recommendation,
                BlacklistHit   = result.BlacklistHit
            };

            // --- NUEVO: persistencia del analisis en la tabla Analyses ---
            var entity = new AnalysisEntity
            {
                UserId         = userId,
                TypeId         = 2, // 2 = "URL" segun seed data de AnalysisTypeEntity
                InputContent   = model.Url,
                Verdict        = result.Verdict,
                RiskScore      = (byte)result.RiskScore,
                Signals        = string.Join(", ", result.Signals),
                Recommendation = result.Recommendation
            };

            _context.Analyses.Add(entity);
            await _context.SaveChangesAsync();
            // --- FIN NUEVO ---

            return View("Result", vm);
        }
    }
}