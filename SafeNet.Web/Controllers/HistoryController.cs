// SafeNet.Web/Controllers/HistoryController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SafeNet.Data;
using SafeNet.Web.Models.ViewModels;
using System.Security.Claims;
using System.Text;

namespace SafeNet.Web.Controllers
{
    [Authorize]
    public class HistoryController : Controller
    {
        private readonly AppDbContext _context;
        private const int PageSize = 10;

        public HistoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /History
        public async Task<IActionResult> Index(
            string? filterVerdict,
            string? filterType,
            string? filterDate,
            string? search,
            int page = 1)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

            var query = _context.Analyses
                .Include(a => a.AnalysisType)
                .Where(a => a.UserId == userId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filterVerdict))
                query = query.Where(a => a.Verdict == filterVerdict);

            if (!string.IsNullOrEmpty(filterType))
                query = query.Where(a => a.AnalysisType != null &&
                                         a.AnalysisType.TypeName == filterType);

            if (!string.IsNullOrEmpty(filterDate))
            {
                var now = DateTime.UtcNow;
                query = filterDate switch
                {
                    "today" => query.Where(a => a.CreatedAt.Date == now.Date),
                    "week"  => query.Where(a => a.CreatedAt >= now.AddDays(-7)),
                    "month" => query.Where(a => a.CreatedAt >= now.AddDays(-30)),
                    _       => query
                };
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                string term = search.Trim().ToLower();
                query = query.Where(a =>
                    (a.InputContent != null    && a.InputContent.ToLower().Contains(term))    ||
                    (a.Signals      != null    && a.Signals.ToLower().Contains(term))          ||
                    (a.Recommendation != null  && a.Recommendation.ToLower().Contains(term))   ||
                    (a.Verdict      != null    && a.Verdict.ToLower().Contains(term)));
            }

            int total = await query.CountAsync();

            if (page < 1) page = 1;

            var analyses = await query
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var allUserAnalyses = await _context.Analyses
                .Where(a => a.UserId == userId)
                .ToListAsync();

            var vm = new HistoryViewModel
            {
                Analyses       = analyses,
                CurrentPage    = page,
                TotalPages     = (int)Math.Ceiling((double)total / PageSize),
                FilterVerdict  = filterVerdict,
                FilterType     = filterType,
                FilterDate     = filterDate,
                Search         = search,
                TotalAnalyses  = allUserAnalyses.Count,
                TotalScams     = allUserAnalyses.Count(a => a.Verdict == "ESTAFA"),
                TotalSuspect   = allUserAnalyses.Count(a => a.Verdict == "SOSPECHOSA"),
                TotalSafe      = allUserAnalyses.Count(a => a.Verdict == "SEGURA"),
                TotalFiltered  = total,
            };

            return View(vm);
        }

        // GET: /History/Detail/{id}
        public async Task<IActionResult> Detail(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

            var analysis = await _context.Analyses
                .Include(a => a.AnalysisType)
                .FirstOrDefaultAsync(a => a.AnalysisId == id && a.UserId == userId);

            if (analysis == null)
                return NotFound();

            return View(analysis);
        }

        // GET: /History/ExportCsv
        public async Task<IActionResult> ExportCsv(
            string? filterVerdict,
            string? filterType,
            string? filterDate,
            string? search)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

            var query = _context.Analyses
                .Include(a => a.AnalysisType)
                .Where(a => a.UserId == userId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filterVerdict))
                query = query.Where(a => a.Verdict == filterVerdict);

            if (!string.IsNullOrEmpty(filterType))
                query = query.Where(a => a.AnalysisType != null &&
                                         a.AnalysisType.TypeName == filterType);

            if (!string.IsNullOrEmpty(filterDate))
            {
                var now = DateTime.UtcNow;
                query = filterDate switch
                {
                    "today" => query.Where(a => a.CreatedAt.Date == now.Date),
                    "week"  => query.Where(a => a.CreatedAt >= now.AddDays(-7)),
                    "month" => query.Where(a => a.CreatedAt >= now.AddDays(-30)),
                    _       => query
                };
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                string term = search.Trim().ToLower();
                query = query.Where(a =>
                    (a.InputContent   != null && a.InputContent.ToLower().Contains(term))   ||
                    (a.Signals        != null && a.Signals.ToLower().Contains(term))         ||
                    (a.Recommendation != null && a.Recommendation.ToLower().Contains(term))  ||
                    (a.Verdict        != null && a.Verdict.ToLower().Contains(term)));
            }

            var analyses = await query
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("ID,Tipo,Contenido,Veredicto,RiesgoScore,Senales,Recomendacion,Fecha");

            foreach (var a in analyses)
            {
                string content   = $"\"{a.InputContent?.Replace("\"", "''") ?? ""}\"";
                string signals   = $"\"{a.Signals?.Replace("\"", "''") ?? ""}\"";
                string recommend = $"\"{a.Recommendation?.Replace("\"", "''") ?? ""}\"";

                sb.AppendLine(
                    $"{a.AnalysisId}," +
                    $"{a.AnalysisType?.TypeName ?? "Desconocido"}," +
                    $"{content},{a.Verdict},{a.RiskScore}," +
                    $"{signals},{recommend}," +
                    $"{a.CreatedAt:yyyy-MM-dd HH:mm:ss}");
            }

            byte[] bytes    = Encoding.UTF8.GetBytes(sb.ToString());
            string fileName = $"safenet_historial_{DateTime.UtcNow:yyyyMMdd_HHmm}.csv";

            return File(bytes, "text/csv", fileName);
        }

        // POST: /History/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

            var analysis = await _context.Analyses
                .FirstOrDefaultAsync(a => a.AnalysisId == id && a.UserId == userId);

            if (analysis != null)
            {
                _context.Analyses.Remove(analysis);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
