// SafeNet.Web/Models/ViewModels/HistoryViewModel.cs
// REEMPLAZAR el archivo existente con este contenido (agrega Search y TotalFiltered)
using SafeNet.Data.Entidades;

namespace SafeNet.Web.Models.ViewModels
{
    public class HistoryViewModel
    {
        // ── Lista de análisis paginada ────────────────────────────────────────
        public List<AnalysisEntity> Analyses { get; set; } = new();

        // ── Paginación ────────────────────────────────────────────────────────
        public int CurrentPage  { get; set; } = 1;
        public int TotalPages   { get; set; } = 1;

        // ── Filtros activos ───────────────────────────────────────────────────
        public string? FilterVerdict { get; set; }
        public string? FilterType    { get; set; }
        public string? FilterDate    { get; set; }
        public string? Search        { get; set; }   // NUEVO v9 — búsqueda por texto

        // ── Estadísticas globales del usuario ─────────────────────────────────
        public int TotalAnalyses { get; set; }
        public int TotalScams    { get; set; }
        public int TotalSuspect  { get; set; }
        public int TotalSafe     { get; set; }
        public int TotalFiltered { get; set; }       // NUEVO v9 — resultados con filtros activos

        // ── Helpers ───────────────────────────────────────────────────────────
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage     => CurrentPage < TotalPages;
        public bool HasActiveFilters =>
            !string.IsNullOrEmpty(FilterVerdict) ||
            !string.IsNullOrEmpty(FilterType)    ||
            !string.IsNullOrEmpty(FilterDate)    ||
            !string.IsNullOrEmpty(Search);
    }
}
