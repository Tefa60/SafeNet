using SafeNet.Core.Models;

namespace SafeNet.Core.Interfaces
{
    public interface IAnalysisService
    {
        Task<AnalysisResult> AnalizarTextoAsync(string texto, string usuarioId);
    }
}