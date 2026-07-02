using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SafeNet.Data;
var builder = WebApplication.CreateBuilder(args);

// Conexion a base de datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("No se encontró la cadena de conexión.");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
// Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<AppDbContext>();
// Registrar servicios de SafeNet
builder.Services.AddHttpClient<SafeNet.Core.Services.ClaudeApiService>();
builder.Services.AddScoped<SafeNet.Core.Services.AnalysisService>();
builder.Services.AddScoped<SafeNet.Core.Interfaces.IAnalysisService, SafeNet.Core.Services.AnalysisService>();
builder.Services.AddScoped<SafeNet.Core.Services.OcrCheckerService>();
builder.Services.AddControllersWithViews();
var app = builder.Build();

Console.WriteLine("=== DIAGNOSTICO LEPTONICA ===");
string[] leptRoots = new[] { "/usr", "/lib" };
foreach (var root in leptRoots)
{
    try
    {
        foreach (var f in System.IO.Directory.EnumerateFiles(root, "*lept*", System.IO.SearchOption.AllDirectories))
        {
            Console.WriteLine("[LEPT] " + f);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("[LEPT] Error en " + root + ": " + ex.Message);
    }
}
Console.WriteLine("=== FIN DIAGNOSTICO LEPTONICA ===");

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();