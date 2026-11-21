using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class VacaConfiguration : IEntityTypeConfiguration<Vaca>
    {
        public void Configure(EntityTypeBuilder<Vaca> builder)
        {
            builder.ToTable("Vacas");
            builder.HasKey(v => v.Id);
            builder.Property(v => v.Nombre).IsRequired().HasMaxLength(100);
            builder.Property(v => v.Raza).IsRequired().HasMaxLength(100);
        }
    }
}
