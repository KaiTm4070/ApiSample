using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    /// <remarks>
    /// Los filtros y órdenes sobre fechas y tipo se hacen en SQL. Las condiciones y sumas sobre
    /// cantidades (decimal) se hacen en memoria porque SQLite no las traduce; un producto tiene pocos lotes.
    /// </remarks>
    public class LoteRepository : Repository<LoteProducto>, ILoteRepository
    {
        public LoteRepository(AppDbContext context) : base(context) { }

        public async Task<IReadOnlyList<LoteProducto>> ObtenerVendiblesAsync(
            TipoProducto tipo, DateOnly fecha, CancellationToken ct = default)
        {
            var lotes = await Context.Lotes
                .Where(l => l.Tipo == tipo && l.FechaVencimiento >= fecha && l.FechaProduccion <= fecha)
                .OrderBy(l => l.FechaProduccion)
                .ThenBy(l => l.Id)
                .ToListAsync(ct);

            return lotes.Where(l => l.EsVendible(fecha)).ToList();
        }

        public async Task<IReadOnlyList<LoteProducto>> ObtenerVencidosAsync(
            DateOnly fecha, CancellationToken ct = default)
        {
            var lotes = await Context.Lotes
                .Where(l => l.FechaVencimiento < fecha)
                .OrderBy(l => l.FechaProduccion)
                .ThenBy(l => l.Id)
                .ToListAsync(ct);

            return lotes.Where(l => l.CantidadDisponible > 0).ToList();
        }

        public async Task<IReadOnlyList<LoteProducto>> ObtenerPorProduccionAsync(
            DateOnly? desde, DateOnly? hasta, TipoProducto? tipo = null, CancellationToken ct = default)
        {
            var consulta = Context.Lotes.AsQueryable();
            if (desde.HasValue) consulta = consulta.Where(l => l.FechaProduccion >= desde.Value);
            if (hasta.HasValue) consulta = consulta.Where(l => l.FechaProduccion <= hasta.Value);
            if (tipo.HasValue) consulta = consulta.Where(l => l.Tipo == tipo.Value);

            return await consulta
                .OrderBy(l => l.FechaProduccion)
                .ThenBy(l => l.Id)
                .ToListAsync(ct);
        }

        public async Task<decimal> LitrosTransformadosAsync(DateOnly fecha, CancellationToken ct = default)
        {
            var litros = await Context.Lotes
                .Where(l => l.FechaProduccion == fecha)
                .Select(l => l.LitrosUsados)
                .ToListAsync(ct);
            return litros.Sum();
        }
    }
}
