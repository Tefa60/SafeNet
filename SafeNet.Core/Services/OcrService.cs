using System.Linq;
using Tesseract;

namespace SafeNet.Core.Services
{
    public class OcrService
    {
        private readonly string _tessDataPath;

        // Rutas conocidas donde Debian/Ubuntu suele instalar tessdata,
        // segun la version de Tesseract que trae el repositorio de paquetes.
        // Se usa la primera que exista en el sistema.
        private static readonly string[] RutasLinuxPosibles = new[]
        {
            "/usr/share/tesseract-ocr/5/tessdata",
            "/usr/share/tesseract-ocr/4.00/tessdata",
            "/usr/share/tesseract-ocr/tessdata",
            "/usr/share/tessdata"
        };

        public OcrService()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                _tessDataPath = RutasLinuxPosibles.FirstOrDefault(Directory.Exists)
                    ?? RutasLinuxPosibles[0];
            }
            else
            {
                _tessDataPath = @"C:\Program Files\Tesseract-OCR\tessdata";
            }
        }

        public string ExtractText(byte[] imageBytes)
        {
            try
            {
                using var engine = new TesseractEngine(_tessDataPath, "spa+eng", EngineMode.Default);
                using var img = Pix.LoadFromMemory(imageBytes);
                using var page = engine.Process(img);
                return page.GetText().Trim();
            }
            catch (Exception ex)
            {
                string innerMsg = ex.InnerException != null ? ex.InnerException.Message : "(sin inner exception)";
                bool existe = Directory.Exists(_tessDataPath);
                string archivos = "(no se pudo listar)";
                try
                {
                    if (existe)
                    {
                        archivos = string.Join(", ", Directory.GetFiles(_tessDataPath).Select(Path.GetFileName));
                        if (string.IsNullOrEmpty(archivos)) archivos = "(directorio vacio)";
                    }
                    else
                    {
                        archivos = "(directorio no existe)";
                    }
                }
                catch (Exception exList)
                {
                    archivos = "(error al listar: " + exList.Message + ")";
                }
                return $"[OCR no disponible: {ex.Message} | Inner: {innerMsg}] (ruta: {_tessDataPath}, existe: {existe}, archivos: {archivos})";
            }
        }
    }
}