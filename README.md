# SafeNet 🛡️
**Detector Inteligente de Estafas Digitales**

Aplicación web ASP.NET Core MVC que utiliza la API de Anthropic Claude para analizar textos, URLs e imágenes en busca de señales de estafa o phishing.

🌐 **Producción:** https://safenet-y09d.onrender.com  
📦 **Repositorio:** https://github.com/Tefa60/SafeNet  
👩‍💻 **Autora:** Dannae Estefany Miranda Tapia

---

## Stack Tecnológico

| Capa | Tecnología |
|------|-----------|
| Lenguaje | C# 12 / .NET 8 |
| Framework | ASP.NET Core MVC 8 + Razor |
| Base de datos local | SQL Server 2022 Express |
| Base de datos producción | PostgreSQL en Supabase |
| ORM | Entity Framework Core 8 (Code-First) |
| Autenticación | ASP.NET Identity + Roles (Admin / User) |
| IA | Anthropic Claude API — claude-sonnet-4-20250514 |
| OCR | Tesseract 5.2.0 (spa + eng) |
| UI | Bootstrap 5 + Bootstrap Icons + Chart.js 4.4 |
| Contenedor | Docker |
| Despliegue | Render.com — Web Service (Docker, Free) |

---

## Módulos

### 🔍 Análisis de Texto
Detecta patrones de estafa en mensajes, correos o conversaciones usando IA.  
Ruta: `/` (página principal)

### 🔗 Verificación de URL
Comprueba si una URL es segura, sospechosa o una estafa conocida.  
Incluye lista negra local + análisis por IA.  
Ruta: `/Url`

### 🖼️ Análisis de Imagen
Extrae texto de imágenes (OCR con Tesseract) y lo analiza con IA.  
Ruta: `/Image`

### 📋 Mi Historial
Vista del historial de análisis del usuario autenticado.  
Filtros por veredicto, tipo y fecha. Exportación a CSV. Vista de detalle individual.  
Ruta: `/History`

### 👤 Mi Perfil
Gestión de cuenta: cambio de contraseña.  
Ruta: `/Account/Profile`

### 🔐 Autenticación
Registro, inicio de sesión y cierre de sesión con ASP.NET Identity.  
Bloqueo tras 5 intentos fallidos.  
Rutas: `/Account/Login`, `/Account/Register`

### 🛡️ Panel de Administración
Estadísticas globales con gráficos (Chart.js). Listado de todos los análisis.  
Solo accesible para usuarios con rol `Admin`.  
Ruta: `/Admin`

---

## Estructura del Proyecto

```
SafeNet/
├── SafeNet.Core/
│   ├── Interfaces/
│   │   └── IAnalysisService.cs
│   └── Services/
│       ├── AnalysisService.cs
│       ├── ClaudeApiService.cs
│       ├── OcrService.cs
│       └── UrlCheckerService.cs
├── SafeNet.Data/
│   ├── Entidades/
│   │   ├── AnalysisEntity.cs
│   │   ├── AnalysisTypeEntity.cs
│   │   └── ScamPatternEntity.cs
│   ├── Migrations/
│   └── AppDbContext.cs
├── SafeNet.Web/
│   ├── Controllers/
│   │   ├── AccountController.cs
│   │   ├── AdminController.cs
│   │   ├── AnalysisController.cs
│   │   ├── HistoryController.cs
│   │   ├── HomeController.cs
│   │   ├── ImageController.cs
│   │   └── UrlController.cs
│   ├── Models/ViewModels/
│   ├── Views/
│   │   ├── Account/    (Login, Register, AccessDenied, Profile)
│   │   ├── Admin/
│   │   ├── Analysis/
│   │   ├── History/    (Index, Detail)
│   │   ├── Home/
│   │   ├── Image/
│   │   ├── Shared/
│   │   └── Url/
│   └── Program.cs
├── SafeNet.Tests/
│   ├── AnalysisServiceTests.cs    (8 pruebas)
│   ├── UrlCheckerServiceTests.cs  (12 pruebas)
│   ├── ClaudeApiServiceTests.cs   (4 pruebas)
│   └── SafeNet.Tests.csproj
└── Dockerfile
```

---

## Configuración Local

### Requisitos
- Visual Studio 2022
- .NET 8 SDK
- SQL Server 2022 Express
- Tesseract OCR instalado en `C:/Program Files/Tesseract-OCR/`

### Variables de entorno / appsettings
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=SafeNet;..."
  },
  "AnthropicApiKey": "sk-ant-api03-..."
}
```

### Comandos de base de datos
```bash
# Package Manager Console (SafeNet.Web como proyecto de inicio)
Add-Migration NombreMigracion -Project SafeNet.Data -StartupProject SafeNet.Web
Update-Database -Project SafeNet.Data -StartupProject SafeNet.Web
```

### Credenciales de administrador por defecto
```
Email:    admin@safenet.com
Password: Admin@12345
```
> ⚠️ Cambiar antes del primer deploy a producción.

---

## Deploy en Render

1. Conectar el repositorio en [render.com](https://render.com) como Web Service → Docker
2. Agregar variables de entorno en Render > Environment:

| Variable | Descripción |
|----------|-------------|
| `ConnectionStrings__DefaultConnection` | Cadena de conexión PostgreSQL (Supabase) |
| `ANTHROPIC_API_KEY` | API Key de Anthropic |
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ASPNETCORE_URLS` | `http://+:10000` |

---

## Pruebas

```bash
# Ejecutar todos los tests
dotnet test SafeNet.Tests/SafeNet.Tests.csproj

# Con detalle
dotnet test --logger "console;verbosity=detailed"
```

**Cobertura de pruebas:**
- `AnalysisServiceTests` — 8 pruebas (guardar, filtrar, eliminar, buscar por ID)
- `UrlCheckerServiceTests` — 12 pruebas (validación, lista negra, patrones, integración Claude)
- `ClaudeApiServiceTests` — 4 pruebas (modo simulación, estructura JSON, manejo de errores)

---

## Veredictos posibles

| Veredicto | Significado |
|-----------|-------------|
| 🟢 SEGURA | No se detectaron señales de estafa |
| 🟡 SOSPECHOSA | Algunas señales de alerta, proceder con precaución |
| 🔴 ESTAFA | Alta probabilidad de ser fraudulento |

---

*SafeNet © 2026 — Dannae Estefany Miranda Tapia*
