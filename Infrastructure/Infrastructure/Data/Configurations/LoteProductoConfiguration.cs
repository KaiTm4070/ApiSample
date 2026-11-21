using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class LoteProductoConfiguration : IEntityTypeConfiguration<LoteProducto>
    {
        public void Configure(EntityTypeBuilder<LoteProducto> builder)
        {
            builder.ToTable("Lotes");
            builder.HasKey(l => l.Id);
            builder.Property(l => l.Tipo).HasConversion<string>().HasMaxLength(20);
            builder.Property(l => l.LitrosUsados).HasPrecision(18, 3);
            builder.Property(l => l.CantidadInicial).HasPrecision(18, 3);
            builder.Property(l => l.CantidadDisponible).HasPrecision(18, 3);
            builder.Property(l => l.CantidadDanada).HasPrecision(18, 3);
            builder.Ignore(l => l.Estado); // se calcula, no se guarda

            // Consultas FIFO: por tipo y fecha de producción.
            builder.HasIndex(l => new { l.Tipo, l.FechaProduccion });
            builder.HasIndex(l => l.FechaVencimiento);
        }
    }
}
