using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProduccionRepository : Repository<ProduccionLeche>, IProduccionRepository
    {
        public ProduccionRepository(AppDbContext context) : base(context) { }

        public Task<bool> ExisteAsync(int vacaId, DateOnly fecha, CancellationToken ct = default)
            => Context.Producciones.AnyAsync(p => p.VacaId == vacaId && p.Fecha == fecha, ct);

        public async Task<decimal> LitrosProducidosAsync(DateOnly fecha, CancellationToken ct = default)
        {
            // La suma se hace en memoria: SQLite guarda decimal como texto y no puede sumarlo en SQL.
            var litros = await Context.Producciones
                .Where(p => p.Fecha == fecha)
                .Select(p => p.Litros)
                .ToListAsync(ct);
            return litros.Sum();
        }

        public async Task<IReadOnlyList<ProduccionLeche>> ObtenerPorRangoAsync(
            DateOnly? desde, DateOnly? hasta, CancellationToken ct = default)
        {
            var consulta = Context.Producciones.Include(p => p.Vaca).AsQueryable();
            if (desde.HasValue) consulta = consulta.Where(p => p.Fecha >= desde.Value);
            if (hasta.HasValue) consulta = consulta.Where(p => p.Fecha <= hasta.Value);

            return await consulta
                .OrderBy(p => p.Fecha)
                .ThenBy(p => p.VacaId)
                .ToListAsync(ct);
        }
    }
}
