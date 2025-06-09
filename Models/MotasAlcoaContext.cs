
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using motasAlcoafinal.Models;

namespace MotasAlcoafinal.Models
{
    public class MotasAlcoaContext : IdentityDbContext<ApplicationUser>
    {
        public MotasAlcoaContext(DbContextOptions<MotasAlcoaContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if (!optionsBuilder.IsConfigured)
            
                //optionsBuilder.UseSqlServer("Server=tcp:servidormotasalcoa.database.windows.net,1433;Database=motasAlcoa;User Id=motasAlcoa;Password=THEWEB123#;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;", 
                //    sqlOptions => sqlOptions.EnableRetryOnFailure());
            

            optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));

        }
        public DbSet<Clientes> Clientes { get; set; }
        public DbSet<Pecas> Pecas { get; set; }
        public DbSet<Motocicletas> Motocicletas { get; set; }
        public DbSet<Servicos> Servicos { get; set; }
        public DbSet<ServicoPecas> ServicoPecas { get; set; }
        public DbSet<Encomendas> Encomendas { get; set; }
        public DbSet<EncomendaPecas> EncomendaPecas { get; set; }
    }
}
