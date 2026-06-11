// SafeNet.Core/Services/OcrService.cs
using Tesseract;

namespace SafeNet.Core.Services
{
    public class OcrService
    {
        private readonly string _tessDataPath;

        public OcrService()
        {
            // En produccion (Docker/Render) Tesseract queda en /usr/share/tesseract-ocr/5/tessdata
            // En local (Windows) ajusta a tu ruta de instalacion
            _tessDataPath = Environment.OSVersion.Platform == PlatformID.Unix
                ? "/usr/share/tesseract-ocr/5/tessdata"
                : @"C:\Program Files\Tesseract-OCR\tessdata";
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
                return $"[OCR no disponible: {ex.Message}]";
            }
        }
    }
}
