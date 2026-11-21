using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces
{
    public interface ILoteRepository : IRepository<LoteProducto>
    {
        /// <summary>Lotes con existencias y no vencidos a esa fecha, por FechaProduccion ascendente (base del FIFO).</summary>
        Task<IReadOnlyList<LoteProducto>> ObtenerVendiblesAsync(
            TipoProducto tipo, DateOnly fecha, CancellationToken ct = default);

        /// <summary>Lotes vencidos a esa fecha que todavía tienen existencias (candidatos a pasar a dañado).</summary>
        Task<IReadOnlyList<LoteProducto>> ObtenerVencidosAsync(DateOnly fecha, CancellationToken ct = default);

        /// <summary>Lotes producidos en el período, opcionalmente de un solo tipo (reportes de producto).</summary>
        Task<IReadOnlyList<LoteProducto>> ObtenerPorProduccionAsync(
            DateOnly? desde, DateOnly? hasta, TipoProducto? tipo = null, CancellationToken ct = default);

        /// <summary>Litros ya transformados en lotes de esa fecha de producción (para RN-02).</summary>
        Task<decimal> LitrosTransformadosAsync(DateOnly fecha, CancellationToken ct = default);
    }
}
