using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class ProduccionLecheConfiguration : IEntityTypeConfiguration<ProduccionLeche>
    {
        public void Configure(EntityTypeBuilder<ProduccionLeche> builder)
        {
            builder.ToTable("Producciones");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Litros).HasPrecision(18, 3);

            builder.HasOne(p => p.Vaca)
                .WithMany()
                .HasForeignKey(p => p.VacaId)
                .OnDelete(DeleteBehavior.Restrict);

            // RN-01: una sola producción por vaca y fecha (la base de datos lo garantiza aunque falle la validación).
            builder.HasIndex(p => new { p.VacaId, p.Fecha }).IsUnique();
            builder.HasIndex(p => p.Fecha);
        }
    }
}
