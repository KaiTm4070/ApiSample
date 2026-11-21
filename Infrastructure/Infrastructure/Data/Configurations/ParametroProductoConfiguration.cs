using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class ParametroProductoConfiguration : IEntityTypeConfiguration<ParametroProducto>
    {
        public void Configure(EntityTypeBuilder<ParametroProducto> builder)
        {
            builder.ToTable("ParametrosProducto");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Tipo).HasConversion<string>().HasMaxLength(20);
            builder.Property(p => p.LitrosPorUnidad).HasPrecision(18, 3);
            builder.Property(p => p.Precio).HasPrecision(18, 2);

            // Un solo juego de parámetros por producto y granja.
            builder.HasIndex(p => new { p.GranjaId, p.Tipo }).IsUnique();
        }
    }
}
