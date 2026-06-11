// SafeNet.Web/Controllers/UrlController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeNet.Core.Services;
using SafeNet.Web.Models.ViewModels;
using System.Security.Claims;

namespace SafeNet.Web.Controllers
{
    public class UrlController : Controller
    {
        private readonly UrlCheckerService _urlChecker;

        public UrlController(UrlCheckerService urlChecker)
        {
            _urlChecker = urlChecker;
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

            return View("Result", vm);
        }
    }
}
