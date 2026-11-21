using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Granja> Granjas => Set<Granja>();
        public DbSet<Vaca> Vacas => Set<Vaca>();
        public DbSet<ParametroProducto> ParametrosProducto => Set<ParametroProducto>();
        public DbSet<ProduccionLeche> Producciones => Set<ProduccionLeche>();
        public DbSet<LoteProducto> Lotes => Set<LoteProducto>();
        public DbSet<Venta> Ventas => Set<Venta>();
        public DbSet<VentaDetalle> VentaDetalles => Set<VentaDetalle>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Toma todas las IEntityTypeConfiguration<T> de este ensamblado (carpeta Configurations).
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
