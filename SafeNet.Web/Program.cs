using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SafeNet.Data;
var builder = WebApplication.CreateBuilder(args);
// Conexión a base de datos
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
builder.Services.AddScoped<SafeNet.Core.Services.UrlCheckerService>();
builder.Services.AddScoped<SafeNet.Core.Services.OcrService>();
builder.Services.AddControllersWithViews();
var app = builder.Build();
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

