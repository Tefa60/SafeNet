// SafeNet.Web/Controllers/HomeController.cs
// REEMPLAZAR el archivo existente
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SafeNet.Data;
using SafeNet.Web.Models.ViewModels;
using System.Security.Claims;

namespace SafeNet.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new HomeViewModel
            {
                TotalAnalyses = await _context.Analyses.CountAsync(),
                TotalScams    = await _context.Analyses.CountAsync(a => a.Verdict == "ESTAFA"),
                TotalSafe     = await _context.Analyses.CountAsync(a => a.Verdict == "SEGURA"),
            };

            if (User.Identity?.IsAuthenticated == true)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
                vm.UserAnalyses = await _context.Analyses
                    .Where(a => a.UserId == userId)
                    .CountAsync();
                vm.UserScams = await _context.Analyses
                    .Where(a => a.UserId == userId && a.Verdict == "ESTAFA")
                    .CountAsync();
            }

            return View(vm);
        }
    }
}
