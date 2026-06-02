using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SafeNet.Data.Entidades;

namespace SafeNet.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<AnalysisEntity> Analyses { get; set; }
        public DbSet<AnalysisTypeEntity> AnalysisTypes { get; set; }
        public DbSet<ScamPatternEntity> ScamPatterns { get; set; }
        public DbSet<ReportedUrlEntity> ReportedUrls { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AnalysisTypeEntity>().HasData(
                new AnalysisTypeEntity { TypeId = 1, TypeName = "Texto" },
                new AnalysisTypeEntity { TypeId = 2, TypeName = "URL" },
                new AnalysisTypeEntity { TypeId = 3, TypeName = "Imagen" }
            );
        }
    }
}