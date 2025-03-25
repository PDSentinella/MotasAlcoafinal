
using Microsoft.EntityFrameworkCore;
using motasAlcoa.Models;

namespace MotasAlcoafinal.Models
{
    public class MotasAlcoaContext : DbContext
    {
        public MotasAlcoaContext(DbContextOptions<MotasAlcoaContext> options) : base(options) { }

        public DbSet<Clientes> Clientes { get; set; }
        public DbSet<Pecas> Pecas { get; set; }
        public DbSet<Motocicletas> Motocicletas { get; set; }
        public DbSet<Servicos> Servicos { get; set; }
        public DbSet<ServicoPecas> ServicoPecas { get; set; }
        public DbSet<Encomendas> Encomendas { get; set; }
        public DbSet<EncomendaPecas> EncomendaPecas { get; set; }
    }
}
