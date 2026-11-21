using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class GranjaConfiguration : IEntityTypeConfiguration<Granja>
    {
        public void Configure(EntityTypeBuilder<Granja> builder)
        {
            builder.ToTable("Granjas");
            builder.HasKey(g => g.Id);
            builder.Property(g => g.Nombre).IsRequired().HasMaxLength(100);
            builder.Property(g => g.LitrosPorVaca).HasPrecision(18, 3);
            builder.Ignore(g => g.ProduccionDiariaEsperada);

            // Las colecciones del dominio son de solo lectura: EF llena los campos _vacas y _parametros.
            builder.HasMany(g => g.Vacas)
                .WithOne()
                .HasForeignKey(v => v.GranjaId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(g => g.Vacas).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(g => g.Parametros)
                .WithOne()
                .HasForeignKey(p => p.GranjaId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(g => g.Parametros).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
