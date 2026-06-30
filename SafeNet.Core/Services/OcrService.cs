using System.Linq;
using Tesseract;

namespace SafeNet.Core.Services
{
    public class OcrService
    {
        private readonly string _tessDataPath;

        // Rutas conocidas donde Debian/Ubuntu suele instalar tessdata via apt-get,
        // segun la version de Tesseract que traiga el repositorio de paquetes.
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
                // En produccion (Docker/Render): probar cada ruta conocida y usar la primera que exista
                _tessDataPath = RutasLinuxPosibles.FirstOrDefault(Directory.Exists)
                    ?? RutasLinuxPosibles[0]; // si ninguna existe, se deja la primera como valor por defecto
                                              // (el error real se vera reflejado en el catch de ExtractText)
            }
            else
            {
                // En local (Windows)
                _tessDataPath = @"C:\Program Files\Tesseract-OCR\tessdata";
            }
        }

        public string ExtractText(byte[] imageBytes)
        {
            try
            {
                using var engine = new TesseractEngine(_tessDataPath, "spa+eng", EngineMode.Default);
                using var img    = Pix.LoadFromMemory(imageBytes);
                using var page   = engine.Process(img);
                return page.GetText().Trim();
            }
            catch (Exception ex)
            {
                return $"[OCR no disponible: {ex.Message} (ruta usada: {_tessDataPath})]";
            }
        }
    }
}