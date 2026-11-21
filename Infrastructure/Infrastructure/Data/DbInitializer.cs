using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public static class DbInitializer
    {
        /// <summary>
        /// Si la base está vacía crea la granja con los valores sugeridos del taller.
        /// Después se editan con PUT /api/granja: no se vuelven a sobrescribir.
        /// </summary>
        public static async Task SembrarAsync(AppDbContext context, CancellationToken ct = default)
        {
            if (await context.Granjas.AnyAsync(ct)) return;

            context.Granjas.Add(Granja.CrearConValoresSugeridos());
            await context.SaveChangesAsync(ct);
        }
    }
}
