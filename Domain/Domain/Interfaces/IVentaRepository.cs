using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces
{
    public interface IVentaRepository : IRepository<Venta>
    {
        /// <summary>Una venta con su detalle de lotes cargado (Find no carga las colecciones).</summary>
        Task<Venta?> ObtenerConDetalleAsync(int id, CancellationToken ct = default);

        /// <summary>Ventas del período con su detalle cargado.</summary>
        Task<IReadOnlyList<Venta>> ObtenerPorRangoAsync(
            DateOnly? desde, DateOnly? hasta, TipoProducto? tipo = null, CancellationToken ct = default);
    }
}
