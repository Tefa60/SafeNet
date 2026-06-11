// SafeNet.Web/Controllers/AdminController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SafeNet.Data;
using SafeNet.Web.Models.ViewModels;

namespace SafeNet.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var analyses = await _context.Analyses
                .Include(a => a.AnalysisType)
                .OrderByDescending(a => a.CreatedAt)
                .Take(100)
                .ToListAsync();

            var vm = new AdminViewModel
            {
                TotalAnalyses  = await _context.Analyses.CountAsync(),
                TotalScams     = await _context.Analyses.CountAsync(a => a.Verdict == "ESTAFA"),
                TotalSuspect   = await _context.Analyses.CountAsync(a => a.Verdict == "SOSPECHOSA"),
                TotalSafe      = await _context.Analyses.CountAsync(a => a.Verdict == "SEGURA"),
                TotalUsers     = await _context.Users.CountAsync(),
                RecentAnalyses = analyses
            };

            return View(vm);
        }
    }
}
