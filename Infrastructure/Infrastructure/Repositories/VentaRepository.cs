using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class VentaRepository : Repository<Venta>, IVentaRepository
    {
        public VentaRepository(AppDbContext context) : base(context) { }

        public Task<Venta?> ObtenerConDetalleAsync(int id, CancellationToken ct = default)
            => Context.Ventas.Include(v => v.Detalles).FirstOrDefaultAsync(v => v.Id == id, ct);

        public async Task<IReadOnlyList<Venta>> ObtenerPorRangoAsync(
            DateOnly? desde, DateOnly? hasta, TipoProducto? tipo = null, CancellationToken ct = default)
        {
            var consulta = Context.Ventas.Include(v => v.Detalles).AsQueryable();
            if (desde.HasValue) consulta = consulta.Where(v => v.Fecha >= desde.Value);
            if (hasta.HasValue) consulta = consulta.Where(v => v.Fecha <= hasta.Value);
            if (tipo.HasValue) consulta = consulta.Where(v => v.Tipo == tipo.Value);

            return await consulta
                .OrderBy(v => v.Fecha)
                .ThenBy(v => v.Id)
                .ToListAsync(ct);
        }
    }
}
