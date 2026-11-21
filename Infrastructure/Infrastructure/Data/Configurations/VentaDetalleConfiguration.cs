using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class VentaDetalleConfiguration : IEntityTypeConfiguration<VentaDetalle>
    {
        public void Configure(EntityTypeBuilder<VentaDetalle> builder)
        {
            builder.ToTable("VentaDetalles");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Cantidad).HasPrecision(18, 3);

            builder.HasOne(d => d.Lote)
                .WithMany()
                .HasForeignKey(d => d.LoteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
